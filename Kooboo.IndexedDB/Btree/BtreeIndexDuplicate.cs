//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;

namespace Kooboo.IndexedDB.Btree
{
    /// <summary>
    /// This is to save the duplicate key items within a btree, in order to make sure that Btree does not contains duplicate key for better search performance. 
    /// When a btree index with unique = false, it allows save duplicate key item with different block position. 
    /// However, the blockposition record must be unique. 
    /// </summary>
    public class BtreeIndexDuplicate
    {
        //this design use 4 jump linked table, each sub containns 10 records.  See end of this file for design. 
        // this might not be the best efficient search that use 10 sub records. It is only a guess now, best performance has not been tested. use of 50% chance upgrade theory when inserting in the jumb table theory is not effecitive. 

        private string fullfilename;

        public static int sectionlen = 46;

        public static int indicatorindex = 2;

        private int counterStartindex = 20;   /// the counter of each level. 
                                              /// 
        private Int64 startRecordPointerPositionIndex = 3;

        private Int64 totalCounterPositionIndex = 12;

        // private int levelstartindex = 7;

        private int maxJumpTableLevel = 4;

        public static byte startbyteone = 10;
        public static byte startbytetwo = 13;

        private int counterBeforePromotion = 10;

        private FileStream _indexstream;
        private HashSet<Int64> _freespace;

        private object _object = new object();

        public BtreeIndexDuplicate(string indexfullfilename)
        {
            this.fullfilename = indexfullfilename;

            _freespace = new HashSet<long>();
        }

        public void OpenOrCreate()
        {
            // create the file and write a header. 
            if (!File.Exists(this.fullfilename))
            {
                // file not exists.first check directory exists or not.
                string dirname = Path.GetDirectoryName(this.fullfilename);
                if (!System.IO.Directory.Exists(dirname))
                {
                    System.IO.Directory.CreateDirectory(dirname);
                }

                string headertext = "store btree duplicate items, do not modify";
                byte[] headerbytes = System.Text.Encoding.ASCII.GetBytes(headertext);

                byte[] firstsector = new byte[sectionlen];

                System.Buffer.BlockCopy(headerbytes, 0, firstsector, 0, headerbytes.Length);

                FileStream indexFileStream = new FileStream(this.fullfilename, FileMode.Create, FileAccess.Write);
                indexFileStream.Write(firstsector, 0, sectionlen);

                indexFileStream.Close();
            }

        }

        public bool Exists
        {
            get
            {
                return File.Exists(this.fullfilename);
            }
        }

        /// <summary>
        /// First create two duplicate, when there is a need to use duplicate, it must always start with two same key records. 
        /// </summary>
        /// <param name="positionx">The block file position x</param>
        /// <param name="positiony">The block file position y</param>
        public Int64 AddFirst(Int64 positionx, Int64 positiony)
        {
            Int64 sectionposition = createNewStartSection();

            Add(sectionposition, positionx);
            Add(sectionposition, positiony);

            return sectionposition;
        }

        public bool Add(Int64 sectionStartPosition, Int64 blockposition)
        {
            //if (getIndicator(sectionStartPosition) != enumSectionType.StartSection)
            //{
            //    throw new Exception("wrong start section");
            //}

            JumpRecord startSearchRecord = getStartPointerRecord(sectionStartPosition);

            Dictionary<int, JumpRecord> loadChain = new Dictionary<int, JumpRecord>();

            JumpRecord currentRecord = startSearchRecord;

            JumpRecord previousrecord = currentRecord;

            while (true)
            {
                if (currentRecord == null)
                {
                    return false;
                }
                if (blockposition == currentRecord.BlockPosition)
                {
                    // record already exists. 
                    return false;
                }

                else if (blockposition > currentRecord.BlockPosition)
                {
                    if (currentRecord.Next > 0)
                    {
                        previousrecord = currentRecord;
                        currentRecord = getJumpRecord(currentRecord.Next);
                        continue;
                    }
                    else if (currentRecord.Buttom > 0)
                    {
                        int currentlevel = currentRecord.level;
                        loadChain.Add(currentlevel, currentRecord);
                        previousrecord = currentRecord;
                        currentRecord = getJumpRecord(currentRecord.Buttom);
                        currentRecord.level = (byte)(currentlevel + 1);
                        continue;
                    }
                    else
                    {
                        // found
                        previousrecord = currentRecord;
                        break;
                    }
                }
                else
                {
                    if (previousrecord.Buttom > 0)
                    {
                        int currentlevel = previousrecord.level;
                        loadChain.Add(currentlevel, previousrecord);
                        currentRecord = getJumpRecord(previousrecord.Buttom);
                        if (currentRecord == null)
                        {
                            continue;
                        }
                        currentRecord.level = (byte)(currentlevel + 1);
                        //this currentRecord has the same block position as previous record,
                        //so blockposition must > currentRecord.blockPosition;
                        continue;
                    }
                    else
                    {
                        //found
                        break;
                    }
                }

            }

            // now we should insert the block position after this previous record. 
            JumpRecord newrecord = insertNewRecord(blockposition);
            updateNavigator(previousrecord.diskLocation, newrecord.diskLocation, enumPosition.next);
            updateNavigator(newrecord.diskLocation, previousrecord.diskLocation, enumPosition.previous);

            if (previousrecord.Next > 0)
            {
                updateNavigator(newrecord.diskLocation, previousrecord.Next, enumPosition.next);
                updateNavigator(previousrecord.Next, newrecord.diskLocation, enumPosition.previous);
            }

            // now determine whether to promote the record or not. 

            int counter = getLevelCounter(sectionStartPosition, previousrecord.level);

            if (counter < this.counterBeforePromotion)
            {
                counter = counter + 1;
                setLevelCounter(sectionStartPosition, (byte)counter, previousrecord.level);
            }
            else
            {
                // we need to promote this item. 
                promoteItems(sectionStartPosition, loadChain, blockposition);
                reloadStartRecordPointer(sectionStartPosition);
            }

            this.updateTotalCounter(sectionStartPosition, 1);
            return true;
        }

        public bool HasKey(Int64 sectionStartPosition, Int64 blockposition)
        {
            if (getIndicator(sectionStartPosition) != enumSectionType.StartSection)
            {
                throw new Exception("wrong start section");
            }
            JumpRecord startSearchRecord = getStartPointerRecord(sectionStartPosition);

            JumpRecord currentRecord = startSearchRecord;

            JumpRecord previousrecord = currentRecord;

            while (true)
            {
                if (blockposition == currentRecord.BlockPosition)
                {
                    return true;
                }

                else if (blockposition > currentRecord.BlockPosition)
                {
                    if (currentRecord.Next > 0)
                    {
                        previousrecord = currentRecord;
                        currentRecord = getJumpRecord(currentRecord.Next);
                        continue;
                    }
                    else if (currentRecord.Buttom > 0)
                    {
                        previousrecord = currentRecord;
                        currentRecord = getJumpRecord(currentRecord.Buttom);

                        continue;
                    }
                    else
                    {
                        //reach end, not found. 
                        return false;
                    }
                }
                else
                {
                    if (previousrecord.Buttom > 0)
                    {

                        currentRecord = getJumpRecord(previousrecord.Buttom);

                        continue;
                    }
                    else
                    {
                        //reach end, not found. 
                        return false;
                    }
                }

            }


        }

        private void reloadStartRecordPointer(Int64 sectionStartPointer)
        {

            Int64 position = 0;

            for (int i = 0; i < this.maxJumpTableLevel; i++)
            {
                position = sectionStartPointer + i * sectionlen + 46;  //46 is the start 46 bytes for counter, etc. 
                byte[] nextpointer = new byte[8];

                lock (_object)
                {
                    IndexStream.Position = position + 11;   //warning, 11 is the next pointer position, it might change.
                    IndexStream.Read(nextpointer, 0, 8);
                }

                if (BitConverter.ToInt64(nextpointer, 0) > 0)
                {
                    break;
                }
            }

            lock (_object)
            {
                IndexStream.Position = sectionStartPointer + this.startRecordPointerPositionIndex;
                IndexStream.Write(BitConverter.GetBytes(position), 0, 8);
            }
        }

        public bool Del(Int64 SectionStartPosition, Int64 blockPosition)
        {

            JumpRecord startSearchRecord = getStartPointerRecord(SectionStartPosition);

            if (startSearchRecord == null)
            {
                return false;
            }

            JumpRecord currentRecord = startSearchRecord;

            JumpRecord previousrecord = currentRecord;

            while (currentRecord != null)
            {
                if (blockPosition == currentRecord.BlockPosition)
                {
                    // record found, delete it. 
                    deleteRecord(currentRecord);

                    this.updateTotalCounter(SectionStartPosition, -1);
                    return true;
                }

                else if (blockPosition > currentRecord.BlockPosition)
                {
                    if (currentRecord.Next > 0)
                    {
                        previousrecord = currentRecord;
                        currentRecord = getJumpRecord(currentRecord.Next);
                        continue;
                    }
                    else if (currentRecord.Buttom > 0)
                    {
                        previousrecord = currentRecord;
                        currentRecord = getJumpRecord(currentRecord.Buttom);

                        continue;
                    }
                    else
                    {
                        //reach end, not found. 
                        return false;
                    }
                }
                else
                {
                    if (previousrecord.Buttom > 0)
                    {

                        currentRecord = getJumpRecord(previousrecord.Buttom);
                        continue;
                    }
                    else
                    {
                        //reach end, not found. 
                        return false;
                    }
                }

            }

            return false;
        }

        public List<Int64> Get(Int64 SectionStartPosition, int skip, int take)
        {
            JumpRecord start = getStartPointerRecord(SectionStartPosition);
            while (start.Buttom > 0)
            {
                start = getJumpRecord(start.Buttom);
            }

            if (start.Next <= 0)
            {
                return null;
            }

            int skipcount = 0;
            int takecount = 0;

            List<Int64> recordlist = new List<Int64>();

            while (start.Next > 0)
            {
                start = getJumpRecord(start.Next);

                if (skipcount < skip)
                {
                    skipcount += 1;
                    continue;
                }


                recordlist.Add(start.BlockPosition);

                takecount += 1;

                if (takecount >= take)
                {
                    break;
                }

            }

            return recordlist;

        }

        public Int64 GetOne(Int64 SectionStartPosition)
        {
            JumpRecord start = getStartPointerRecord(SectionStartPosition);
            while (start.Buttom > 0)
            {
                start = getJumpRecord(start.Buttom);
            }

            if (start.Next <= 0)
            {
                return 0;
            }
            else
            {
                start = getJumpRecord(start.Next);

                return start.BlockPosition;
            }
        }

        public List<Int64> GetAll(Int64 SectionStartPosition)
        {
            JumpRecord start = getStartPointerRecord(SectionStartPosition);
            while (start.Buttom > 0)
            {
                start = getJumpRecord(start.Buttom);
            }

            if (start.Next <= 0)
            {
                return null;
            }

            List<Int64> recordlist = new List<Int64>();

            while (start.Next > 0)
            {
                start = getJumpRecord(start.Next);
                recordlist.Add(start.BlockPosition);
            }

            return recordlist;

        }

        public List<long> GetSome(Int64 SectionStartPosition, int takecount)
        {

            JumpRecord start = getStartPointerRecord(SectionStartPosition);
            while (start.Buttom > 0)
            {
                start = getJumpRecord(start.Buttom);
            }

            if (start.Next <= 0)
            {
                return null;
            }

            List<long> recordlist = new List<long>();

            int counter = 0;

            while (start.Next > 0)
            {
                start = getJumpRecord(start.Next);
                recordlist.Add(start.BlockPosition);
                counter += 1;
                if (counter >= takecount)
                {
                    return recordlist;
                }
            }

            return recordlist;

        }

        public BtreeIndexDuplicateCollection getCollection(Int64 SectionStartPosition)
        {

            JumpRecord start = getStartPointerRecord(SectionStartPosition);
            while (start.Buttom > 0)
            {
                start = getJumpRecord(start.Buttom);
            }

            BtreeIndexDuplicateCollection collection = new BtreeIndexDuplicateCollection(this, start);

            return collection;

        }

        public BtreeIndexDuplicateReader getReader(Int64 SectionStartPosition)
        {
            JumpRecord start = getStartPointerRecord(SectionStartPosition);
            while (start != null && start.Buttom > 0)
            {
                start = getJumpRecord(start.Buttom);
            }

            BtreeIndexDuplicateReader reader = new BtreeIndexDuplicateReader(this, start);

            return reader;
        }

        public int count(Int64 SectionStartPosition)
        {
            return getTotalCounter(SectionStartPosition);

            //JumpRecord start = getStartPointerRecord(SectionStartPosition);

            //while (start.Buttom > 0)
            //{
            //    start = getJumpRecord(start.Buttom);
            //}

            //int counter = 0;

            //while (start.Next > 0)
            //{
            //    start = getJumpRecord(start.Next);

            //    counter += 1;

            //}

            //return counter;
        }

        /// <summary>
        /// used to determine that there are more than one record so that it can NOT be removed yet. 
        /// </summary>
        /// <param name="SectionStartPosition"></param>
        /// <returns></returns>
        public bool hasMoreThanOne(Int64 SectionStartPosition)
        {
            JumpRecord start = getStartPointerRecord(SectionStartPosition);

            while (start.Buttom > 0)
            {
                start = getJumpRecord(start.Buttom);
            }

            int counter = 0;

            while (start.Next > 0)
            {
                start = getJumpRecord(start.Next);

                counter += 1;

                if (counter > 1)
                {
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// delete the record and all the chains. 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private void deleteRecord(JumpRecord record)
        {

            updateNavigator(record.Previous, record.Next, enumPosition.next);
            updateNavigator(record.Next, record.Previous, enumPosition.previous);
            markAsDeleted(record.diskLocation);

            JumpRecord toprecord = record;

            JumpRecord buttomreocrd = record;

            while (toprecord.TOP > 0)
            {
                toprecord = getJumpRecord(toprecord.TOP);

                updateNavigator(toprecord.Previous, toprecord.Next, enumPosition.next);
                updateNavigator(toprecord.Next, toprecord.Previous, enumPosition.previous);

                markAsDeleted(toprecord.diskLocation);

            }

            while (buttomreocrd.Buttom > 0)
            {
                buttomreocrd = getJumpRecord(buttomreocrd.Buttom);


                updateNavigator(buttomreocrd.Previous, buttomreocrd.Next, enumPosition.next);
                updateNavigator(buttomreocrd.Next, buttomreocrd.Previous, enumPosition.previous);

                markAsDeleted(buttomreocrd.diskLocation);

            }



        }

        private void markAsDeleted(Int64 diskposition)
        {
            Int64 fixedposition = diskposition + 2;
            lock (_object)
            {
                IndexStream.Position = fixedposition;
                IndexStream.WriteByte((byte)enumSectionType.DeletedAvailable);
            }
            _freespace.Add(diskposition);
        }

        private JumpRecord insertNewRecord(Int64 blockposition)
        {
            JumpRecord record = new JumpRecord();
            record.BlockPosition = blockposition;

            record.Indicator = enumSectionType.Record;

            Int64 insertposition = getInsertPosition();

            lock (_object)
            {
                IndexStream.Position = insertposition;
                IndexStream.Write(record.ToBytes(), 0, sectionlen);
            }

            record.diskLocation = insertposition;
            return record;
        }

        private void updateNavigator(Int64 recorddiskposition, Int64 PointerDiskPosition, enumPosition position)
        {
            int relativeposition = 0;
            switch (position)
            {
                case enumPosition.next:
                    relativeposition = 11;
                    break;

                case enumPosition.previous:
                    relativeposition = 3;
                    break;

                case enumPosition.top:
                    relativeposition = 19;
                    break;

                case enumPosition.bottom:
                    relativeposition = 27;
                    break;
                default:
                    relativeposition = 11;
                    break;
            }

            lock (_object)
            {
                IndexStream.Position = recorddiskposition + relativeposition;
                IndexStream.Write(BitConverter.GetBytes(PointerDiskPosition), 0, 8);
            }

        }


        /// <summary>
        /// promote items up. 
        /// </summary>
        /// <param name="sectionStartPosition"></param>
        /// <param name="loadchains"></param>
        /// <param name="blockposition"></param>
        private void promoteItems(Int64 sectionStartPosition, Dictionary<int, JumpRecord> loadchains, Int64 blockposition)
        {
            /// we now use a temp counter on each level, and reset that temp counter when item promoted. 
            foreach (var item in loadchains.OrderByDescending(o => o.Key))
            {
                int level = item.Key;

                JumpRecord newrecord = insertNewRecord(blockposition);

                JumpRecord ItemCurrentLevel = item.Value;

                updateNavigator(ItemCurrentLevel.diskLocation, newrecord.diskLocation, enumPosition.next);
     
                updateNavigator(newrecord.diskLocation, ItemCurrentLevel.diskLocation, enumPosition.previous);

                if (ItemCurrentLevel.Next > 0)
                {
                    updateNavigator(ItemCurrentLevel.Next, newrecord.diskLocation, enumPosition.previous);
                    updateNavigator(newrecord.diskLocation, ItemCurrentLevel.Next, enumPosition.next);
                }

                int counter = getLevelCounter(sectionStartPosition, level);
                counter = counter + 1;
                if (counter < this.counterBeforePromotion)
                {
                    setLevelCounter(sectionStartPosition, (byte)counter, level);
                    //STOP, not promot up again. 
                    return;
                }
                else
                {
                    // reSet counter and continue loop up. 
                    setLevelCounter(sectionStartPosition, (byte)0, level);
                }
            }
        }


        private Int64 createNewStartSection()
        {
            /// in the format of 
            /// [8] start search pointer. Pointer to the highest jump table. 
            /// [2] * = number of counter of each level.  start pointer = 

            // header section + number levels. 
            int totallen = sectionlen + this.maxJumpTableLevel * sectionlen;

            byte[] totalbytes = new byte[totallen];

            byte[] header = new byte[sectionlen];

            header[0] = startbyteone;
            header[1] = startbytetwo;

            header[44] = startbyteone;
            header[45] = startbytetwo;

            header[2] = (byte)enumSectionType.StartSection;

            List<JumpRecord> tablelist = new List<JumpRecord>();

            for (int i = 0; i < this.maxJumpTableLevel; i++)
            {
                JumpRecord newrecord = new JumpRecord();
                newrecord.BlockPosition = Int64.MinValue;
                newrecord.Indicator = enumSectionType.Record;
                newrecord.level = (byte)i;

                tablelist.Add(newrecord);
            }

            for (int i = 0; i < tablelist.Count; i++)
            {
                if (i == 0)
                {
                    updateNavigator(tablelist[i].diskLocation, tablelist[i + 1].diskLocation, enumPosition.next);

                }
                else if (i == tablelist.Count - 1)
                {
                    updateNavigator(tablelist[i].diskLocation, tablelist[i - 1].diskLocation, enumPosition.previous);
                }
                else
                {
                    // in the middle. 
                    updateNavigator(tablelist[i].diskLocation, tablelist[i - 1].diskLocation, enumPosition.previous);
                    updateNavigator(tablelist[i].diskLocation, tablelist[i + 1].diskLocation, enumPosition.next);
                }
            }


            Int64 writeposition = getInsertPosition();

            //write position for each start of level table. 
            for (int i = 0; i < tablelist.Count; i++)
            {
                tablelist[i].diskLocation = writeposition + 46 + i * 46;
            }

            Int64 lastitemindex = tablelist[tablelist.Count - 1].diskLocation;
            System.Buffer.BlockCopy(BitConverter.GetBytes(lastitemindex), 0, header, 3, 8);

            lock (_object)
            {
                IndexStream.Position = writeposition;
                IndexStream.Write(header, 0, sectionlen);

                foreach (var item in tablelist)
                {
                    IndexStream.Position = item.diskLocation;
                    IndexStream.Write(item.ToBytes(), 0, sectionlen);
                }

            }

            return writeposition;
        }

        private void checkfreespace()
        {
            //TODO:
        }


        private Int64 getInsertPosition()
        {

            Int64 pos;
            //if (this._freespace.Count > 0)
            //{
            //    pos = this._freespace.FirstOrDefault();
            //    this._freespace.Remove(pos);
            //}
            //else
            //{
                pos = IndexStream.Length;
                Int64 left = pos % sectionlen;

                if (left != 0)
                {
                    pos = pos - left;
                }
           // }

            /// Check to make sure that POS is the right spot.
            return pos;
        }

        public FileStream IndexStream
        {
            get
            {
                if (_indexstream == null || _indexstream.CanRead == false)
                {
                    lock (_object)
                    {
                        if (_indexstream == null || _indexstream.CanRead == false)
                        {
                            _indexstream = StreamManager.GetFileStream(this.fullfilename); 
                        }
                    }

                }
                return _indexstream;
            }
        }

        public void Close()
        {
            if (_indexstream != null)
            {
                lock (_object)
                {
                    if (_indexstream != null)
                    {
                        _indexstream.Close();
                        _indexstream = null;
                    }
                }
            }
        }

        public void Flush()
        {
            if (_indexstream != null)
            {
                lock (_object)
                {
                    if (_indexstream != null)
                    {
                        _indexstream.Flush();
                    }
                }
            }
        }

        public void DelSelf()
        {
            lock (_object)
            {
                Close();
                File.Delete(this.fullfilename);
            }
        }

        /// <summary>
        /// get the indicator of this block of 46 bytes. 
        /// </summary>
        /// <param name="startSectionPosition"></param>
        /// <returns></returns>
        private enumSectionType getIndicator(Int64 startSectionPosition)
        {
            int indicatorbyte;

            lock (_object)
            {
                IndexStream.Position = startSectionPosition + BtreeIndexDuplicate.indicatorindex;
                indicatorbyte = IndexStream.ReadByte();
            }

            return (enumSectionType)indicatorbyte;
        }

        private JumpRecord getStartPointerRecord(Int64 sectionStartPosition)
        {
            byte[] pointerbytes = new byte[8];
            lock (_object)
            {
                IndexStream.Position = sectionStartPosition + this.startRecordPointerPositionIndex;

                IndexStream.Read(pointerbytes, 0, 8);
            }

            return getJumpRecord(BitConverter.ToInt64(pointerbytes, 0));
        }

        public JumpRecord getJumpRecord(Int64 sectionposition)
        {
            byte[] recordbyte = new byte[46];

            lock (_object)
            {
                IndexStream.Position = sectionposition;
                IndexStream.Read(recordbyte, 0, 46);
            }

            JumpRecord record = new JumpRecord();
            record.pointerBytes = recordbyte;

            record.diskLocation = sectionposition;

            if (record.Indicator == enumSectionType.DeletedAvailable)
            {
                return null;
            }

            return record;
        }

        private byte getLevelCounter(Int64 startSectionPosition, int level)
        {
            if (level < 0)
            {
                level = 0;
            }

            Int64 readposition = startSectionPosition + level * 2 + this.counterStartindex;
            int x;

            lock (_object)
            {
                IndexStream.Position = readposition;
                x = IndexStream.ReadByte();
            }

            return (byte)x;
        }

        private void setLevelCounter(Int64 startSectionPosition, byte counter, int level)
        {
            if (level < 0)
            {
                level = 0;
            }

            Int64 readposition = startSectionPosition + level * 2 + this.counterStartindex;

            lock (_object)
            {
                IndexStream.Position = readposition;
                IndexStream.WriteByte(counter);
            }
        }

        private void updateTotalCounter(Int64 startSectionPosition, int PlusOrMinusDigit)
        {

            Int64 readposition = startSectionPosition + this.totalCounterPositionIndex;


            byte[] counterbyte = new byte[4];

            lock (_object)
            {
                IndexStream.Position = readposition;
                IndexStream.Position = readposition;
                IndexStream.Read(counterbyte, 0, 4);
            }

            int counter = BitConverter.ToInt32(counterbyte, 0);

            counter = counter + PlusOrMinusDigit;

            counterbyte = BitConverter.GetBytes(counter);

            lock (_object)
            {
                IndexStream.Position = readposition;
                IndexStream.Write(counterbyte, 0, 4);
            }

        }

        private int getTotalCounter(Int64 startSectionPosition)
        {

            byte[] counterbyte = new byte[4];

            lock (_object)
            {

                Int64 readposition = startSectionPosition + this.totalCounterPositionIndex;
                IndexStream.Position = readposition;

                IndexStream.Position = readposition;
                IndexStream.Read(counterbyte, 0, 4);

                return BitConverter.ToInt32(counterbyte, 0);
            }

        }

    }

    /// <summary>
    /// Deleted = avaiable for another insertion. 
    /// </summary>
    public enum enumSectionType
    {
        DeletedAvailable = 0,
        StartSection = 2,
        Record = 3
    }

    public enum enumPosition
    {
        next = 0,
        previous = 1,
        top = 2,
        bottom = 3
    }

}

/// Bytes format as below. 

//start

//[2] startbyte.
//[1]indicator
//[4] counter.
//[8][1] level1  position + tempcounter.  when tempcounter >=10, upgrade the level and set tempcounter =0.
//[8][1] level2  position + tempcounter
//[8][1] level3  position + tempcounter.
//[8][1] level4  position + tempcounter
//[2] endbyte

//45 bytes total


//each

//[2] startbyte
//[1] indicator (deleted, start, record.). 
//[8] previous
//[8] next
//[8] top
//[8] buttom
//[8] blockposition
//[2] endbyte


//45 bytes.

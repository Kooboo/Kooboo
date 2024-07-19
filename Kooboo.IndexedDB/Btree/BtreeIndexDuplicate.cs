//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.IndexedDB.BTree
{
    /// <summary>
    /// This is to save the duplicate key items within a bTree, in order to make sure that BTree does not contains duplicate key for better search performance. 
    /// When a bTree index with unique = false, it allows save duplicate key item with different block position. 
    /// However, the blockPosition record must be unique. 
    /// </summary>
    public class BTreeIndexDuplicate
    {
        //this design use 4 jump linked table, each sub contains 10 records.  See end of this file for design. 
        // this might not be the best efficient search that use 10 sub records. It is only a guess now, best performance has not been tested. use of 50% chance upgrade theory when inserting in the jump table theory is not effective. 

        private string fullFileName;

        public static int sectionLen = 46;

        public static int indicatorIdex = 2;

        private int counterStartIndex = 20;   /// the counter of each level. 
                                              /// 
        private Int64 startRecordPointerPositionIndex = 3;

        private Int64 totalCounterPositionIndex = 12;


        private int maxJumpTableLevel = 4;

        public static byte startByteOne = 10;
        public static byte startByteTwo = 13;

        private int counterBeforePromotion = 10;

        private FileStream _indexStream;
        private HashSet<Int64> _freeSpace;

        private object _object = new object();

        public BTreeIndexDuplicate(string IndexFullFileName)
        {
            this.fullFileName = IndexFullFileName;

            _freeSpace = new HashSet<long>();
        }

        public void OpenOrCreate()
        {
            // create the file and write a header. 
            if (!File.Exists(this.fullFileName))
            {
                // file not exists.first check directory exists or not.
                string dirName = Path.GetDirectoryName(this.fullFileName);
                if (!System.IO.Directory.Exists(dirName))
                {
                    System.IO.Directory.CreateDirectory(dirName);
                }

                string headerText = "store btree duplicate items, do not modify";
                byte[] headerBytes = System.Text.Encoding.ASCII.GetBytes(headerText);

                byte[] firstSector = new byte[sectionLen];

                System.Buffer.BlockCopy(headerBytes, 0, firstSector, 0, headerBytes.Length);

                FileStream indexFileStream = new(this.fullFileName, FileMode.Create, FileAccess.Write);
                indexFileStream.Write(firstSector, 0, sectionLen);

                indexFileStream.Close();
            }

        }

        public bool Exists
        {
            get
            {
                return File.Exists(this.fullFileName);
            }
        }

        /// <summary>
        /// First create two duplicate, when there is a need to use duplicate, it must always start with two same key records. 
        /// </summary>
        /// <param name="PositionX">The block file position x</param>
        /// <param name="PositionY">The block file position y</param>
        public Int64 AddFirst(Int64 PositionX, Int64 PositionY)
        {
            Int64 sectionPosition = createNewStartSection();

            Add(sectionPosition, PositionX);
            Add(sectionPosition, PositionY);

            return sectionPosition;
        }

        public bool Add(Int64 sectionStartPosition, Int64 blockPosition)
        {
            //if (getIndicator(sectionStartPosition) != enumSectionType.StartSection)
            //{
            //    throw new Exception("wrong start section");
            //}

            JumpRecord startSearchRecord = getStartPointerRecord(sectionStartPosition);

            Dictionary<int, JumpRecord> loadChain = new Dictionary<int, JumpRecord>();

            JumpRecord currentRecord = startSearchRecord;

            JumpRecord previousRecord = currentRecord;

            while (true)
            {
                if (currentRecord == null)
                {
                    return false;
                }
                if (blockPosition == currentRecord.BlockPosition)
                {
                    // record already exists. 
                    return false;
                }

                else if (blockPosition > currentRecord.BlockPosition)
                {
                    if (currentRecord.Next > 0)
                    {
                        previousRecord = currentRecord;
                        currentRecord = getJumpRecord(currentRecord.Next);
                        continue;
                    }
                    else if (currentRecord.Bottom > 0)
                    {
                        int currentLevel = currentRecord.level;
                        loadChain.Add(currentLevel, currentRecord);
                        previousRecord = currentRecord;
                        currentRecord = getJumpRecord(currentRecord.Bottom);
                        currentRecord.level = (byte)(currentLevel + 1);
                        continue;
                    }
                    else
                    {
                        // found
                        previousRecord = currentRecord;
                        break;
                    }
                }
                else
                {
                    if (previousRecord.Bottom > 0)
                    {
                        int currentLevel = previousRecord.level;
                        loadChain.Add(currentLevel, previousRecord);
                        currentRecord = getJumpRecord(previousRecord.Bottom);
                        if (currentRecord == null)
                        {
                            continue;
                        }
                        currentRecord.level = (byte)(currentLevel + 1);
                        //this currentRecord has the same block position as previous record,
                        //so blockPosition must > currentRecord.blockPosition;
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
            JumpRecord newRecord = insertNewRecord(blockPosition);
            updateNavigator(previousRecord.diskLocation, newRecord.diskLocation, enumPosition.next);
            updateNavigator(newRecord.diskLocation, previousRecord.diskLocation, enumPosition.previous);

            if (previousRecord.Next > 0)
            {
                updateNavigator(newRecord.diskLocation, previousRecord.Next, enumPosition.next);
                updateNavigator(previousRecord.Next, newRecord.diskLocation, enumPosition.previous);
            }

            // now determine whether to promote the record or not. 

            int counter = getLevelCounter(sectionStartPosition, previousRecord.level);

            if (counter < this.counterBeforePromotion)
            {
                counter++;
                setLevelCounter(sectionStartPosition, (byte)counter, previousRecord.level);
            }
            else
            {
                // we need to promote this item. 
                promoteItems(sectionStartPosition, loadChain, blockPosition);
                reloadStartRecordPointer(sectionStartPosition);
            }

            this.updateTotalCounter(sectionStartPosition, 1);
            return true;
        }

        public bool HasKey(Int64 sectionStartPosition, Int64 blockPosition)
        {
            if (getIndicator(sectionStartPosition) != enumSectionType.StartSection)
            {
                throw new Exception("wrong start section");
            }
            JumpRecord startSearchRecord = getStartPointerRecord(sectionStartPosition);

            JumpRecord currentRecord = startSearchRecord;

            JumpRecord previousRecord = currentRecord;

            while (true)
            {
                if (blockPosition == currentRecord.BlockPosition)
                {
                    return true;
                }

                else if (blockPosition > currentRecord.BlockPosition)
                {
                    if (currentRecord.Next > 0)
                    {
                        previousRecord = currentRecord;
                        currentRecord = getJumpRecord(currentRecord.Next);
                        continue;
                    }
                    else if (currentRecord.Bottom > 0)
                    {
                        previousRecord = currentRecord;
                        currentRecord = getJumpRecord(currentRecord.Bottom);

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
                    if (previousRecord.Bottom > 0)
                    {

                        currentRecord = getJumpRecord(previousRecord.Bottom);

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
                position = sectionStartPointer + i * sectionLen + 46;  //46 is the start 46 bytes for counter, etc. 
                byte[] nextPointer = new byte[8];

                lock (_object)
                {
                    IndexStream.Position = position + 11;   //warning, 11 is the next pointer position, it might change.
                    IndexStream.Read(nextPointer, 0, 8);
                }

                if (BitConverter.ToInt64(nextPointer, 0) > 0)
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

            JumpRecord previousRecord = currentRecord;

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
                        previousRecord = currentRecord;
                        currentRecord = getJumpRecord(currentRecord.Next);
                        continue;
                    }
                    else if (currentRecord.Bottom > 0)
                    {
                        previousRecord = currentRecord;
                        currentRecord = getJumpRecord(currentRecord.Bottom);

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
                    if (previousRecord.Bottom > 0)
                    {

                        currentRecord = getJumpRecord(previousRecord.Bottom);
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
            while (start.Bottom > 0)
            {
                start = getJumpRecord(start.Bottom);
            }

            if (start.Next <= 0)
            {
                return null;
            }

            int skipCount = 0;
            int takeCount = 0;

            List<Int64> recordList = new List<Int64>();

            while (start.Next > 0)
            {
                start = getJumpRecord(start.Next);

                if (skipCount < skip)
                {
                    skipCount += 1;
                    continue;
                }


                recordList.Add(start.BlockPosition);

                takeCount += 1;

                if (takeCount >= take)
                {
                    break;
                }

            }

            return recordList;

        }

        public Int64 GetOne(Int64 SectionStartPosition)
        {
            JumpRecord start = getStartPointerRecord(SectionStartPosition);
            while (start.Bottom > 0)
            {
                start = getJumpRecord(start.Bottom);
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
            while (start.Bottom > 0)
            {
                start = getJumpRecord(start.Bottom);
            }

            if (start.Next <= 0)
            {
                return null;
            }

            List<Int64> recordList = new List<Int64>();

            while (start.Next > 0)
            {
                start = getJumpRecord(start.Next);
                recordList.Add(start.BlockPosition);
            }

            return recordList;

        }

        public List<long> GetSome(Int64 SectionStartPosition, int takeCount)
        {

            JumpRecord start = getStartPointerRecord(SectionStartPosition);
            while (start.Bottom > 0)
            {
                start = getJumpRecord(start.Bottom);
            }

            if (start.Next <= 0)
            {
                return null;
            }

            List<long> recordList = new List<long>();

            int counter = 0;

            while (start.Next > 0)
            {
                start = getJumpRecord(start.Next);
                recordList.Add(start.BlockPosition);
                counter += 1;
                if (counter >= takeCount)
                {
                    return recordList;
                }
            }

            return recordList;

        }

        public BTreeIndexDuplicateCollection getCollection(Int64 SectionStartPosition)
        {

            JumpRecord start = getStartPointerRecord(SectionStartPosition);
            while (start.Bottom > 0)
            {
                start = getJumpRecord(start.Bottom);
            }

            BTreeIndexDuplicateCollection collection = new BTreeIndexDuplicateCollection(this, start);

            return collection;

        }

        public BTreeIndexDuplicateReader getReader(Int64 SectionStartPosition)
        {
            JumpRecord start = getStartPointerRecord(SectionStartPosition);
            while (start != null && start.Bottom > 0)
            {
                start = getJumpRecord(start.Bottom);
            }

            BTreeIndexDuplicateReader reader = new BTreeIndexDuplicateReader(this, start);

            return reader;
        }

        public int count(Int64 SectionStartPosition)
        {
            return getTotalCounter(SectionStartPosition);

        }

        /// <summary>
        /// used to determine that there are more than one record so that it can NOT be removed yet. 
        /// </summary>
        /// <param name="SectionStartPosition"></param>
        /// <returns></returns>
        public bool hasMoreThanOne(Int64 SectionStartPosition)
        {
            JumpRecord start = getStartPointerRecord(SectionStartPosition);

            while (start.Bottom > 0)
            {
                start = getJumpRecord(start.Bottom);
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

            JumpRecord topRecord = record;

            JumpRecord buttomReocrd = record;

            while (topRecord.TOP > 0)
            {
                topRecord = getJumpRecord(topRecord.TOP);

                updateNavigator(topRecord.Previous, topRecord.Next, enumPosition.next);
                updateNavigator(topRecord.Next, topRecord.Previous, enumPosition.previous);

                markAsDeleted(topRecord.diskLocation);

            }

            while (buttomReocrd.Bottom > 0)
            {
                buttomReocrd = getJumpRecord(buttomReocrd.Bottom);


                updateNavigator(buttomReocrd.Previous, buttomReocrd.Next, enumPosition.next);
                updateNavigator(buttomReocrd.Next, buttomReocrd.Previous, enumPosition.previous);

                markAsDeleted(buttomReocrd.diskLocation);

            }



        }

        private void markAsDeleted(Int64 diskposition)
        {
            Int64 fixedPosition = diskposition + 2;
            lock (_object)
            {
                IndexStream.Position = fixedPosition;
                IndexStream.WriteByte((byte)enumSectionType.DeletedAvailable);
            }
            _freeSpace.Add(diskposition);
        }

        private JumpRecord insertNewRecord(Int64 blockPosition)
        {
            JumpRecord record = new JumpRecord();
            record.BlockPosition = blockPosition;

            record.Indicator = enumSectionType.Record;

            Int64 insertPosition = getInsertPosition();

            lock (_object)
            {
                IndexStream.Position = insertPosition;
                IndexStream.Write(record.ToBytes(), 0, sectionLen);
            }

            record.diskLocation = insertPosition;
            return record;
        }

        private void updateNavigator(Int64 RecordDiskPosition, Int64 PointerDiskPosition, enumPosition position)
        {
            int relativePosition = 0;
            switch (position)
            {
                case enumPosition.next:
                    relativePosition = 11;
                    break;

                case enumPosition.previous:
                    relativePosition = 3;
                    break;

                case enumPosition.top:
                    relativePosition = 19;
                    break;

                case enumPosition.bottom:
                    relativePosition = 27;
                    break;
                default:
                    relativePosition = 11;
                    break;
            }

            lock (_object)
            {
                IndexStream.Position = RecordDiskPosition + relativePosition;
                IndexStream.Write(BitConverter.GetBytes(PointerDiskPosition), 0, 8);
            }

        }


        /// <summary>
        /// promote items up. 
        /// </summary>
        /// <param name="sectionStartPosition"></param>
        /// <param name="LoadedChains"></param>
        /// <param name="BlockPosition"></param>
        private void promoteItems(Int64 sectionStartPosition, Dictionary<int, JumpRecord> LoadedChains, Int64 BlockPosition)
        {
            /// we now use a temp counter on each level, and reset that temp counter when item promoted. 
            foreach (var item in LoadedChains.OrderByDescending(o => o.Key))
            {
                int level = item.Key;

                JumpRecord newRecord = insertNewRecord(BlockPosition);

                JumpRecord ItemCurrentLevel = item.Value;

                updateNavigator(ItemCurrentLevel.diskLocation, newRecord.diskLocation, enumPosition.next);

                updateNavigator(newRecord.diskLocation, ItemCurrentLevel.diskLocation, enumPosition.previous);

                if (ItemCurrentLevel.Next > 0)
                {
                    updateNavigator(ItemCurrentLevel.Next, newRecord.diskLocation, enumPosition.previous);
                    updateNavigator(newRecord.diskLocation, ItemCurrentLevel.Next, enumPosition.next);
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
            int totalLen = sectionLen + this.maxJumpTableLevel * sectionLen;

            byte[] totalBytes = new byte[totalLen];

            byte[] header = new byte[sectionLen];

            header[0] = startByteOne;
            header[1] = startByteTwo;

            header[44] = startByteOne;
            header[45] = startByteTwo;

            header[2] = (byte)enumSectionType.StartSection;

            List<JumpRecord> tableList = new List<JumpRecord>();

            for (int i = 0; i < this.maxJumpTableLevel; i++)
            {
                JumpRecord newRecord = new JumpRecord();
                newRecord.BlockPosition = Int64.MinValue;
                newRecord.Indicator = enumSectionType.Record;
                newRecord.level = (byte)i;

                tableList.Add(newRecord);
            }

            for (int i = 0; i < tableList.Count; i++)
            {
                if (i == 0)
                {
                    updateNavigator(tableList[i].diskLocation, tableList[i + 1].diskLocation, enumPosition.next);

                }
                else if (i == tableList.Count - 1)
                {
                    updateNavigator(tableList[i].diskLocation, tableList[i - 1].diskLocation, enumPosition.previous);
                }
                else
                {
                    // in the middle. 
                    updateNavigator(tableList[i].diskLocation, tableList[i - 1].diskLocation, enumPosition.previous);
                    updateNavigator(tableList[i].diskLocation, tableList[i + 1].diskLocation, enumPosition.next);
                }
            }


            Int64 writePosition = getInsertPosition();

            //write position for each start of level table. 
            for (int i = 0; i < tableList.Count; i++)
            {
                tableList[i].diskLocation = writePosition + 46 + i * 46;
            }

            Int64 lastItemIndex = tableList[tableList.Count - 1].diskLocation;
            System.Buffer.BlockCopy(BitConverter.GetBytes(lastItemIndex), 0, header, 3, 8);

            lock (_object)
            {
                IndexStream.Position = writePosition;
                IndexStream.Write(header, 0, sectionLen);

                foreach (var item in tableList)
                {
                    IndexStream.Position = item.diskLocation;
                    IndexStream.Write(item.ToBytes(), 0, sectionLen);
                }

            }

            return writePosition;
        }

        private void checkFreeSpace()
        {
            //TODO:
        }


        private Int64 getInsertPosition()
        {

            Int64 pos;
            //if (this._freeSpace.Count > 0)
            //{
            //    pos = this._freeSpace.FirstOrDefault();
            //    this._freeSpace.Remove(pos);
            //}
            //else
            //{
            pos = IndexStream.Length;
            Int64 left = pos % sectionLen;

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
                if (_indexStream == null || _indexStream.CanRead == false)
                {
                    lock (_object)
                    {
                        if (_indexStream == null || _indexStream.CanRead == false)
                        {
                            _indexStream = StreamManager.GetFileStream(this.fullFileName);
                        }
                    }

                }
                return _indexStream;
            }
        }

        public void Close()
        {
            if (_indexStream != null)
            {
                lock (_object)
                {
                    if (_indexStream != null)
                    {
                        _indexStream.Close();
                        _indexStream = null;
                    }
                }
            }
        }

        public void Flush()
        {
            if (_indexStream != null)
            {
                lock (_object)
                {
                    if (_indexStream != null)
                    {
                        _indexStream.Flush();
                    }
                }
            }
        }

        public void DelSelf()
        {
            lock (_object)
            {
                Close();
                File.Delete(this.fullFileName);
            }
        }

        /// <summary>
        /// get the indicator of this block of 46 bytes. 
        /// </summary>
        /// <param name="startSectionPosition"></param>
        /// <returns></returns>
        private enumSectionType getIndicator(Int64 startSectionPosition)
        {
            int indicatorByte;

            lock (_object)
            {
                IndexStream.Position = startSectionPosition + BTreeIndexDuplicate.indicatorIdex;
                indicatorByte = IndexStream.ReadByte();
            }

            return (enumSectionType)indicatorByte;
        }

        private JumpRecord getStartPointerRecord(Int64 sectionStartPosition)
        {
            byte[] pointerBytes = new byte[8];
            lock (_object)
            {
                IndexStream.Position = sectionStartPosition + this.startRecordPointerPositionIndex;

                IndexStream.Read(pointerBytes, 0, 8);
            }

            return getJumpRecord(BitConverter.ToInt64(pointerBytes, 0));
        }

        public JumpRecord getJumpRecord(Int64 SectionPosition)
        {
            byte[] recordByte = new byte[46];

            lock (_object)
            {
                IndexStream.Position = SectionPosition;
                IndexStream.Read(recordByte, 0, 46);
            }

            JumpRecord record = new JumpRecord
            {
                pointerBytes = recordByte,
                diskLocation = SectionPosition
            };

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

            Int64 readPosition = startSectionPosition + level * 2 + this.counterStartIndex;
            int x;

            lock (_object)
            {
                IndexStream.Position = readPosition;
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

            Int64 readPosition = startSectionPosition + level * 2 + this.counterStartIndex;

            lock (_object)
            {
                IndexStream.Position = readPosition;
                IndexStream.WriteByte(counter);
            }
        }

        private void updateTotalCounter(Int64 startSectionPosition, int PlusOrMinusDigit)
        {

            Int64 readPosition = startSectionPosition + this.totalCounterPositionIndex;


            byte[] counterByte = new byte[4];

            lock (_object)
            {
                IndexStream.Position = readPosition;
                IndexStream.Position = readPosition;
                IndexStream.Read(counterByte, 0, 4);
            }

            int counter = BitConverter.ToInt32(counterByte, 0);

            counter += PlusOrMinusDigit;

            counterByte = BitConverter.GetBytes(counter);

            lock (_object)
            {
                IndexStream.Position = readPosition;
                IndexStream.Write(counterByte, 0, 4);
            }

        }

        private int getTotalCounter(Int64 startSectionPosition)
        {
            byte[] counterByte = new byte[4];
            lock (_object)
            {
                Int64 readPosition = startSectionPosition + this.totalCounterPositionIndex;
                IndexStream.Position = readPosition;

                IndexStream.Position = readPosition;
                IndexStream.Read(counterByte, 0, 4);

                return BitConverter.ToInt32(counterByte, 0);
            }

        }

    }

    /// <summary>
    /// Deleted = available for another insertion. 
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

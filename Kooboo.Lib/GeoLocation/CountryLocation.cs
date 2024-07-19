using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Lib.GeoLocation
{

    public static class CountryLocation
    {
        static CountryLocation()
        {

            Items = new Dictionary<short, CountryLocationModel>();

            var alllines = CountryLines().Split('\n');

            foreach (var cline in alllines)
            {
                if (cline != null)
                {
                    var line = cline.Trim();
                    string[] segs = line.Split(',');

                    if (segs != null)
                    {
                        if (segs.Length == 4)
                        {
                            CountryLocationModel location = new CountryLocationModel();
                            location.CountryCode = segs[0];
                            location.Continent = segs[1];
                            double latitude;
                            if (double.TryParse(segs[2], out latitude))
                            {
                                location.Latitude = latitude;
                            }

                            double longtitude;
                            if (double.TryParse(segs[3], out longtitude))
                            {
                                location.Longtitude = longtitude;
                            }

                            short intcode = CountryCode.ToShort(location.CountryCode);

                            Items[intcode] = location;

                        }
                        else if (segs.Length > 2)
                        {
                            CountryLocationModel location = new CountryLocationModel();
                            location.CountryCode = segs[0];
                            location.Continent = segs[1];

                            short intcode = CountryCode.ToShort(location.CountryCode);

                            Items[intcode] = location;

                        }
                    }
                }
            }





        }

        // int is the hash of countrycode.
        public static Dictionary<short, CountryLocationModel> Items
        {
            get; set;
        }

        public static CountryLocationModel FindCountryLocation(string CountryCode)
        {
            var intcode = GeoLocation.CountryCode.ToShort(CountryCode);

            if (Items.ContainsKey(intcode))
            {
                return Items[intcode];
            }
            return null;
        }

        public static string FindNearByCountry(string TargetCode, List<string> Available)
        {

            var find = Available.Find(o => o == TargetCode);
            if (find != null && find.Any())
            {
                return find;
            }

            var target = FindCountryLocation(TargetCode);
            if (target == null)
            {
                return Available.First();
            }

            double distance = double.MaxValue;
            string result = null;

            foreach (var item in Available)
            {
                var dest = FindCountryLocation(item);

                if (dest != null)
                {
                    var newdistance = GetDistance(target.Latitude, target.Longtitude, dest.Latitude, dest.Longtitude);

                    if (newdistance < distance)
                    {
                        distance = newdistance;
                        result = item;
                    }
                }
            }

            return result;
        }

        public static double GetDistance(double xLa, double xLong, double yLa, double yLong)
        {
            return Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetDistance(xLa, xLong, yLa, yLong);
        }


        public static string CountryLines()
        {
            var lines = @"AD,EU,42.546245,1.601554
AE,AS,23.424076,53.847818
AF,AS,33.93911,67.709953
AG,NA,17.060816,-61.796428
AI,NA,18.220554,-63.068615
AL,EU,41.153332,20.168331
AM,AS,40.069099,45.038189
AO,AF,-11.202692,17.873887
AQ,AN,-75.250973,-0.071389
AR,SA,-38.416097,-63.616672
AS,OC,-14.270972,-170.132217
AT,EU,47.516231,14.550072
AU,OC,-25.274398,133.775136
AW,NA,12.52111,-69.968338
AX,EU,NULL,NULL
AZ,AS,40.143105,47.576927
BA,EU,43.915886,17.679076
BB,NA,13.193887,-59.543198
BD,AS,23.684994,90.356331
BE,EU,50.503887,4.469936
BF,AF,12.238333,-1.561593
BG,EU,42.733883,25.48583
BH,AS,25.930414,50.637772
BI,AF,-3.373056,29.918886
BJ,AF,9.30769,2.315834
BL,NA,NULL,NULL
BM,NA,32.321384,-64.75737
BN,AS,4.535277,114.727669
BO,SA,-16.290154,-63.588653
BQ,SA,NULL,NULL
BR,SA,-14.235004,-51.92528
BS,NA,25.03428,-77.39628
BT,AS,27.514162,90.433601
BV,AN,-54.423199,3.413194
BW,AF,-22.328474,24.684866
BY,EU,53.709807,27.953389
BZ,NA,17.189877,-88.49765
CA,NA,56.130366,-106.346771
CC,AS,-12.164165,96.870956
CD,AF,-4.038333,21.758664
CF,AF,6.611111,20.939444
CG,AF,-0.228021,15.827659
CH,EU,46.818188,8.227512
CI,AF,7.539989,-5.54708
CK,OC,-21.236736,-159.777671
CL,SA,-35.675147,-71.542969
CM,AF,7.369722,12.354722
CN,AS,35.86166,104.195397
CO,SA,4.570868,-74.297333
CR,NA,9.748917,-83.753428
CU,NA,21.521757,-77.781167
CV,AF,16.002082,-24.013197
CW,SA,NULL,NULL
CX,AS,-10.447525,105.690449
CY,AS,35.126413,33.429859
CZ,EU,49.817492,15.472962
DE,EU,51.165691,10.451526
DJ,AF,11.825138,42.590275
DK,EU,56.26392,9.501785
DM,NA,15.414999,-61.370976
DO,NA,18.735693,-70.162651
DZ,AF,28.033886,1.659626
EC,SA,-1.831239,-78.183406
EE,EU,58.595272,25.013607
EG,AF,26.820553,30.802498
EH,AF,24.215527,-12.885834
ER,AF,15.179384,39.782334
ES,EU,40.463667,-3.74922
ET,AF,9.145,40.489673
FI,EU,61.92411,25.748151
FJ,OC,-16.578193,179.414413
FK,SA,-51.796253,-59.523613
FM,OC,7.425554,150.550812
FO,EU,61.892635,-6.911806
FR,EU,46.227638,2.213749
GA,AF,-0.803689,11.609444
GB,EU,55.378051,-3.435973
GD,NA,12.262776,-61.604171
GE,AS,42.315407,43.356892
GF,SA,3.933889,-53.125782
GG,EU,49.465691,-2.585278
GH,AF,7.946527,-1.023194
GI,EU,36.137741,-5.345374
GL,NA,71.706936,-42.604303
GM,AF,13.443182,-15.310139
GN,AF,9.945587,-9.696645
GP,NA,16.995971,-62.067641
GQ,AF,1.650801,10.267895
GR,EU,39.074208,21.824312
GS,AN,-54.429579,-36.587909
GT,NA,15.783471,-90.230759
GU,OC,13.444304,144.793731
GW,AF,11.803749,-15.180413
GY,SA,4.860416,-58.93018
HK,AS,22.396428,114.109497
HM,AN,-53.08181,73.504158
HN,NA,15.199999,-86.241905
HR,EU,45.1,15.2
HT,NA,18.971187,-72.285215
HU,EU,47.162494,19.503304
ID,AS,-0.789275,113.921327
IE,EU,53.41291,-8.24389
IL,AS,31.046051,34.851612
IM,EU,54.236107,-4.548056
IN,AS,20.593684,78.96288
IO,AS,-6.343194,71.876519
IQ,AS,33.223191,43.679291
IR,AS,32.427908,53.688046
IS,EU,64.963051,-19.020835
IT,EU,41.87194,12.56738
JE,EU,49.214439,-2.13125
JM,NA,18.109581,-77.297508
JO,AS,30.585164,36.238414
JP,AS,36.204824,138.252924
KE,AF,-0.023559,37.906193
KG,AS,41.20438,74.766098
KH,AS,12.565679,104.990963
KI,OC,-3.370417,-168.734039
KM,AF,-11.875001,43.872219
KN,NA,17.357822,-62.782998
KP,AS,40.339852,127.510093
KR,AS,35.907757,127.766922
KW,AS,29.31166,47.481766
KY,NA,19.513469,-80.566956
KZ,AS,48.019573,66.923684
LA,AS,19.85627,102.495496
LB,AS,33.854721,35.862285
LC,NA,13.909444,-60.978893
LI,EU,47.166,9.555373
LK,AS,7.873054,80.771797
LR,AF,6.428055,-9.429499
LS,AF,-29.609988,28.233608
LT,EU,55.169438,23.881275
LU,EU,49.815273,6.129583
LV,EU,56.879635,24.603189
LY,AF,26.3351,17.228331
MA,AF,31.791702,-7.09262
MC,EU,43.750298,7.412841
MD,EU,47.411631,28.369885
ME,EU,42.708678,19.37439
MF,NA,NULL,NULL
MG,AF,-18.766947,46.869107
MH,OC,7.131474,171.184478
MK,EU,41.608635,21.745275
ML,AF,17.570692,-3.996166
MM,AS,21.913965,95.956223
MN,AS,46.862496,103.846656
MO,AS,22.198745,113.543873
MP,OC,17.33083,145.38469
MQ,NA,14.641528,-61.024174
MR,AF,21.00789,-10.940835
MS,NA,16.742498,-62.187366
MT,EU,35.937496,14.375416
MU,AF,-20.348404,57.552152
MV,AS,3.202778,73.22068
MW,AF,-13.254308,34.301525
MX,NA,23.634501,-102.552784
MY,AS,4.210484,101.975766
MZ,AF,-18.665695,35.529562
NA,AF,-22.95764,18.49041
NC,OC,-20.904305,165.618042
NE,AF,17.607789,8.081666
NF,OC,-29.040835,167.954712
NG,AF,9.081999,8.675277
NI,NA,12.865416,-85.207229
NL,EU,52.132633,5.291266
NO,EU,60.472024,8.468946
NP,AS,28.394857,84.124008
NR,OC,-0.522778,166.931503
NU,OC,-19.054445,-169.867233
NZ,OC,-40.900557,174.885971
OM,AS,21.512583,55.923255
PA,NA,8.537981,-80.782127
PE,SA,-9.189967,-75.015152
PF,OC,-17.679742,-149.406843
PG,OC,-6.314993,143.95555
PH,AS,12.879721,121.774017
PK,AS,30.375321,69.345116
PL,EU,51.919438,19.145136
PM,NA,46.941936,-56.27111
PN,OC,-24.703615,-127.439308
PR,NA,18.220833,-66.590149
PS,AS,31.952162,35.233154
PT,EU,39.399872,-8.224454
PW,OC,7.51498,134.58252
PY,SA,-23.442503,-58.443832
QA,AS,25.354826,51.183884
RE,AF,-21.115141,55.536384
RO,EU,45.943161,24.96676
RS,EU,44.016521,21.005859
RU,EU,61.52401,105.318756
RW,AF,-1.940278,29.873888
SA,AS,23.885942,45.079162
SB,OC,-9.64571,160.156194
SC,AF,-4.679574,55.491977
SD,AF,12.862807,30.217636
SE,EU,60.128161,18.643501
SG,AS,1.352083,103.819836
SH,AF,-24.143474,-10.030696
SI,EU,46.151241,14.995463
SJ,EU,77.553604,23.670272
SK,EU,48.669026,19.699024
SL,AF,8.460555,-11.779889
SM,EU,43.94236,12.457777
SN,AF,14.497401,-14.452362
SO,AF,5.152149,46.199616
SR,SA,3.919305,-56.027783
SS,AF,NULL,NULL
ST,AF,0.18636,6.613081
SV,NA,13.794185,-88.89653
SX,SA,NULL,NULL
SY,AS,34.802075,38.996815
SZ,AF,-26.522503,31.465866
TC,NA,21.694025,-71.797928
TD,AF,15.454166,18.732207
TF,AN,-49.280366,69.348557
TG,AF,8.619543,0.824782
TH,AS,15.870032,100.992541
TJ,AS,38.861034,71.276093
TK,OC,-8.967363,-171.855881
TL,AS,-8.874217,125.727539
TM,AS,38.969719,59.556278
TN,AF,33.886917,9.537499
TO,OC,-21.178986,-175.198242
TR,EU,38.963745,35.243322
TT,NA,10.691803,-61.222503
TV,OC,-7.109535,177.64933
TW,AS,23.69781,120.960515
TZ,AF,-6.369028,34.888822
UA,EU,48.379433,31.16558
UG,AF,1.373333,32.290275
UM,OC,,
US,NA,37.09024,-95.712891
UY,SA,-32.522779,-55.765835
UZ,AS,41.377491,64.585262
VA,EU,41.902916,12.453389
VC,NA,12.984305,-61.287228
VE,SA,6.42375,-66.58973
VG,NA,18.420695,-64.639968
VI,NA,18.335765,-64.896335
VN,AS,14.058324,108.277199
VU,OC,-15.376706,166.959158
WF,OC,-13.768752,-177.156097
WS,OC,-13.759029,-172.104629
YE,AS,15.552727,48.516388
YT,AF,-12.8275,45.166244
ZA,AF,-30.559482,22.937506
ZM,AF,-13.133897,27.849332
ZW,AF,-19.015438,29.154857
";

            return lines;

        }
    }

    public class CountryLocationModel
    {
        public string CountryCode { get; set; }
        public string Continent { get; set; }
        public double Latitude { get; set; }

        public double Longtitude { get; set; }
    }


}

Jump table is another index technique. 
It is implemented as many levels of linked data.
search from top till the buttom. 

Example:  

0                  10                        20
0      3    6      10     14       18        20
0   1  3  4 6  7 8 10  11 14  15   18   19   20

search starts from top level, goes down to the buttom. 


Btree duplicate file is a special jump table with each duplicate key has a jump table itself and all located in one file.
We use customized comparer instead of .Net default comparer. 
Because we would like to compare values directly in byte array format, 
in order to have better performance. 

Convert a byte[] back to string and compare them is an very expensive action. 
Even for INT, BitConverter back to INT is more consuming as well.

However for Decimal/Double/Single(float), we havenot implement the byte comparsion yet
because it is floating pointer requires more investigation or welecome other people to implement them.


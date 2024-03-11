 here is the plan
 write unit tests to make sure that the program is working....
 there are two sides to the unit test, the events coming from the command side (handlers), and the read from grpc endpoints

1- testing that giving it higher sequence would return false for each event
2- testing giving it lower sequence would return true for each event
3- foreach event should check that data is changed
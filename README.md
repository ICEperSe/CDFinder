# CDFinder
Custom utility, extended version of standard command line "cd". 
When you enter: "cdf Directory", the program searches for a directory and goes into it.
 If there are several results, provides a choice list.
 Also u can use some kind of patterns:
 
 Input format:	"someDir\otherDir"
								"dir\someDir\otherDir"
							 	...
Stars (*) in input path: test* = testPath, testASD ...
							 test*\dir = test1\dir, testA\dir ...
							 test\dir* = test\dir, test\dir1 ...
							 test*\dir* = test1\dir1, test\dir ...
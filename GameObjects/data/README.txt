I have changed the xml files in here. There are a few notable changes, a few of which you probably have noted.

I have reorganized all the xml files to be like prods. This means that each group of items/enemies are now in one file, for example, 
Abyss of Demon enemies and tiles are located in the "AbyssOfDemons" instead of the original "dat13" or whatever.

The reason I'm taking the time to make this is because there are a few notable things to take notice.

The new xml system gives GREAT organization to whatever you're working on. You no longer need to have a huge (and cluttered) addition.xml.
And of course, there is a downside. ServerEngine seems to allocate more memory FASTER on the newer system than it did on the older system.
You will see in XmlDatas.cs that I have added a reader for each file. This is what causes ServerEngine to allocate more memory. Because it needs to
read each file. Now I know that it had to do this before with the old system, but that was simply a few (5) lines of code that would call on
a dat. The reader could simply read the dat and have the number given to the correct dat. It's hard to explain in text so you should go check
out XmlDatas.cs because I have it explained better there.

Now then, there IS a plus.
The xml files are more organized, yes, at the cost of memory, but I DID notice that the server was running EXTREMELY nice when I first ran it
using this new system. The older system seemed to make the server run a tad slower. I have left all of the old systems code in the source so
if you want to go back you can, just remember to delete all the readers that point to each xml file and to remove each xml from the project.


Running the new system shouldn't be that hard for newer computers, but older computers might want to switch back if they are hosting.
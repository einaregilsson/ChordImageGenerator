#Chord Image Generator

[http://einaregilsson.com/chord-image-generator/](http://einaregilsson.com/chord-image-generator/ "A blog post explaining more about the library")


Chord Image Generator is a small .NET library to generate images of guitar chords. It can display chord boxes, starting frets, barred chords, fingerings and open and muted strings.


![Image of a D Chord](http://chords.einaregilsson.com/D.png?p=xx0232&f=---132&s=3 "D Chord")
![Image of a A Chord](http://chords.einaregilsson.com/A.png?p=x02220&f=--123-&s=3 "A Chord")
![Image of a A Chord](http://chords.einaregilsson.com/A_5.png?p=577655&f=134211&s=3 "A bar Chord")

There is a small example website at [http://chords.einaregilsson.com](http://chords.einaregilsson.com) where you can try different chords and see how they are constructed. But basically it is just done by constructing the right url of the form /chordname/fret-positions/fingerings/size . E.g. the chords above are the following urls:

* [http://chords.einaregilsson.com/D.png?p=xx0232&f=---132&s=3](http://chords.einaregilsson.com/D.png?p=xx0232&f=---132&s=3)
* [http://chords.einaregilsson.com/A.png?p=x02220&f=--123-&s=3](http://chords.einaregilsson.com/A.png?p=x02220&f=--123-&s=3)
* [http://chords.einaregilsson.com/A_5.png?p=577655&f=134211&s=3](http://chords.einaregilsson.com/A_5.png?p=577655&f=134211&s=3)

There is nothing web specific about the actual image generation. It can be saved to any stream so you could just as easily use it in a desktop application.

The source is licensed under the GPL. You are also free to link directly to chords.einaregilsson.com, although I give no guarantees about uptime or reliability.


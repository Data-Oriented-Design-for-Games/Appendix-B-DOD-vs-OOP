This is a simulation, in Unity, of balls bouncing around the screen and colliding with each other. 

The calculation is very simple. Every frame, each ball checks the distance to every other ball. If it's less than their combined radius, a collision happens, and they bounce off each other.

Every 60 frames, we spawn double the balls as last time from a pool. If the frame rate falls below 60fps, it falls back to the last ball count and start spawning again. This way it tries to find the maximum number of balls while remaining at 60fps. 

One simulation is done using Data Oriented Design, where all the data is in arrays. The other simulation is done using Object Oriented Programming, where every ball has its own update function to move it, and a function to check collision with another ball. 

The calculations are identical. The only difference is the layout of the data. 

On an iPhone 16 Pro:
With DOD, we are getting over 6,000 balls. 
With OOP, we are getting around 600. 

So DOD is TEN TIMES FASTER.

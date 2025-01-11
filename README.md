# Take a look at my slime mould sim generator!
This is a revamp of an old unity project re-done in C# (currently as a console app, so not particularly user-friendly right now).
I have always loved slime mould simulations so am happy to finally have a proprly parameterised version!
## Technial details and requirements
This project depends on FFMPEG, as of writing I have included a .Zip of the version used for development in the repo. This will need to be unzipped to the correct directory and should throw an fairly obvious error if it is incorrect.
The algorithm has a long running time so the project leverages [ComputeSharp](https://github.com/Sergio0694/ComputeSharp) for GPU accelleration on the bulk of the logic, then uses multithreading for saving the result. This project will work best on machines that support this. 
Requires Windows 7+.
Requires .Net 8.0.

## My slime mould sim generator!
This is a revamp of an old unity project re-done in C# (and is a console app, so not the most user-friendly).
I have always loved slime mould simulations so am happy to finally have a proprly parameterised version!
**This will also become obselete when the generic simulation renderer is completed.**

The result is compiled into a video under the output directory, once the simulation completes.
https://github.com/user-attachments/assets/fac1037d-94ce-474b-9d24-0ad7b074fd3a


## Technial details and requirements
- Requires Windows 7+.
- Requires .Net 8.0.
- This project depends on FFMPEG, as of writing I have included a .Zip of the version used for development in the repo. This will need to be unzipped to the correct directory and should throw an fairly obvious error if it is incorrect.
- - The algorithm has a long running time so the project leverages [ComputeSharp](https://github.com/Sergio0694/ComputeSharp) for GPU accelleration on the bulk of the logic, then uses multithreading for saving the result. This project will work best on machines that support this. 

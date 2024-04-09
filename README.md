# eDIA LSL

This is a wrapper around the [LSL4Unity](https://github.com/labstreaminglayer/LSL4Unity) repository.  
We provide some additional functionalities (e.g., more advanced or specific sending of event markers) and useful implementations of concepts common to many experiments.

## Table of Contents

- [Installing as a package](#installing-as-a-package)
- [Usage](#usage)

## Installing as a package

There are different ways to achieve this. We recommend to always use Unity's `PackageManager`. Click on the `+` sign (top left corner) and then add the package from 
1. **git** (*recommended*; assumes that you have access rights to this repo on gitlab or github)  
    Use the link to the repository extended by `?path=Assets/com.edia.lsl` (e.g., the full path should be something like:  
    `git@gitlab.gwdg.de:3dia/edia_lsl.git?path=Assets/com.edia.lsl` )  

2. **from disk**:  
    Download the content in [Assets/com.edia.lsl](Assets/com.edia.lsl), unzip, and put into a local directory on your machine. Then install from this location.

## Usage

Check out the demo scenes in [Assets/Samples](Assets/Samples) (or [Assets/com.edia.lsl/Samples~](Assets/com.edia.lsl/Samples~)) and read the according [README](Assets/com.edia.lsl/Samples~).

## Useful resources
1. [LSL4Unity repository](https://github.com/labstreaminglayer/LSL4Unity)
2. [C# interface to LabStreamingLayer](https://github.com/labstreaminglayer/liblsl-Csharp)
3. [LSL Slack channel](https://labstreaminglayer.slack.com/)


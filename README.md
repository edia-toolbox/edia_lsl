<p align="center">
  <img src="./Assets/com.edia.lsl/Editor/Resources/Icons/IconLSL.png" width="128" />
</p>

# EDIA LSL

This is a wrapper around the [LSL4Unity](https://github.com/labstreaminglayer/LSL4Unity) repository.  
We provide some additional functionalities (e.g., more advanced or specific sending of event markers) and useful implementations of concepts common to many experiments.

## Installation

### Dependencies
Make sure you have installed [EDIA Core](https://github.com/edia-toolbox/edia_core.git) and its dependencies. 

### As a package (Unity package manager)
- In Unity open the package manager window → **Window** → **Package Manager**  
- Use `Install from GIT URL`  
- To install:
  - **Latest release**:  
    `https://github.com/edia-toolbox/edia_lsl.git?path=Assets/com.edia.lsl#main`
  - **Specific release** (replace `vX.Y.Z` with a version from the [release list](https://github.com/edia-toolbox/edia_lsl/releases)):  
    `https://github.com/edia-toolbox/edia_lsl.git#vX.Y.Z`
  - **Development version**:  
    `https://github.com/edia-toolbox/edia_lsl.git?path=Assets/com.edia.lsl`
- Hit `ADD`

Unity now starts to download and install the `com.edia.lsl` package. 

### For development
Clone this repository. 

## Usage

Check out the demo scenes in [Assets/Samples](Assets/Samples) (or [Assets/com.edia.lsl/Samples~](Assets/com.edia.lsl/Samples~)) and read the according [README](Assets/com.edia.lsl/Samples~).

## Useful resources
1. [LSL4Unity repository](https://github.com/labstreaminglayer/LSL4Unity)
2. [C# interface to LabStreamingLayer](https://github.com/labstreaminglayer/liblsl-Csharp)
3. [LSL Slack channel](https://labstreaminglayer.slack.com/)

## Credits
If you are using this repository for your research or other public work, please cite:
1. the LSL4Unity repository: https://github.com/labstreaminglayer/LSL4Unity
2. the LSL preprint: https://www.biorxiv.org/content/10.1101/2024.02.13.580071v1
3. the EDIA Toolbox: https://github.com/edia-toolbox/edia_core/

## Contribution
We are happy to receive feedback and contributions. If you want to report a bug, please open a GitHub issue. 
If you have questions or suggestions, please use the [GitHub Discussion board](https://github.com/edia-toolbox/edia_core/discussions). 
If you want to [contribute to EDIA](https://github.com/edia-toolbox/edia_core/blob/dev/CONTRIBUTOR.md), ideally first reach out to us (e.g., via the [Discussion board](https://github.com/edia-toolbox/edia_core/discussions) or [email](mailto:edia.toolbox@gmail.com)), and/or post a pull request.  

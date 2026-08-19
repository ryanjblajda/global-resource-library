# RESOURCE REPOSITORY

sort your modules into folders and allow applications to target a single folder you heathen.


## TOOLS

2 different batch scripts should be dropped into your desired target folder, and will create symlinks of all the files so that as far as your applications are concerned, everything lives in a flat structure, but really you can have the files organized and searchable in the core folders.

## CORE FOLDERS

each of these folders are intended to store submodule repos for the named manufacturer.

### CRESTRON

i have things separated out as follows:

- ir-driver-libary
- module-library
    - the module-library is separated out further:
        - vendor-provided
            - stock vendor (or 3d party programmer) modules provided for use with crestron, separated out by the vendor/programmer
        - customized-developed
            - modules that i wrote from scratch, or modified based on a vendor provided module
                - this allows me to separate out what i changed so that if issues are encountered, reverting to the stock vendor module is easily accomplished.
            - modules are separated out by the functionality suite, or the hardware vendor the module controls

### EXTRON

no real separation at the moment for these. 

### Q-SYS

no real separation required as of yet, all the modules here are ones i've created
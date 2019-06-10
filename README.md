# specrun-log-to-playlist


This is a console application which creates a Visual Studio test playlist from a SpecRun log file.
The result is printed to stdout and can be redirected to a file (the playlist file).

## Usage
specRunLog2Playlist [-s \<testStatusValue>] \<specrunlog>

## Options
    -h, --help                 show this message and exit
    -s, --status               filter on test status Succeeded|Failed|Pending
    -i, --ignoreRetries        ignore retries, otherwise the status of the last
                                test execution will be used when filtering



## Example
./SpecRunLog2Playlist.exe -s Failed ../../SpecRunLogSamples/TestProject1_2targets_noRetries.log > failed-tests.playlist

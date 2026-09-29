# TESTING.md, family "restart", launch 1 of 2. The settings are global and live in the profile's
# settings file. Only a second process shows that the file is what the next launch reads at startup:
# reading it back in the same process would show nothing, the values are still in memory.
#
# The chain is two launches under one hold of the lock:
#   -DepMap wsl-deps.sans-facultatifs.map -Filter '09-restart-write' -Then '10-restart-read'
#
# THIS LAUNCH LEAVES NON-DEFAULT VALUES IN THE SETTINGS FILE ON PURPOSE. The step that does it says so;
# launch 2 puts the defaults back. If the chain is cut after this launch, the profile keeps the values:
# delete Config/Mod_NightChange_NightChangeMod.xml (or the file the log names) by hand.
@restart
Feature: the settings are written for the next launch (1 of 2)

  Scenario: the four settings are set to non-default values and written
    Given the save "test-colony" is loaded
    And Night Change: the settings are set to non-default values and kept for the next launch
    Then no errors were logged

# TESTING.md, family "restart", launch 2 of 2. Refuses to pass when the writer ran in this process. The
# teardown of this launch puts the documented defaults back and writes them, so the profile ends clean.
@restart
Feature: the settings kept by the previous launch are read at startup (2 of 2)

  Scenario: the four values written by launch 1 are the ones loaded
    Given the save "test-colony" is loaded
    Then Night Change: the settings kept by the previous launch are in place
    And no errors were logged

# MOD_SETTINGS.md, "Shortcut integration": in RIMMSQOL, reveal the shortcut, open the same settings,
# hide it again. Driven through the shared steps of PickleTools/RimmsqolSteps (RIMMSQOL's own calls, not
# pixel clicks): RIMMSQOL's own list offers NightChange_Settings, hidden; it reveals it and the bar then
# draws it; the revealed button opens the same page as Mod options; hiding it again empties the bar and
# forgetting the choice leaves nothing in RIMMSQOL's file.
#
# WHAT THIS DOES NOT DO: it does not click RIMMSQOL's checkbox; the steps call the calls the checkbox
# makes. Restart persistence of RIMMSQOL's choice is not covered (Night Change stores nothing of it).
#
# Played only by the pass "avec-rimmsqol": without RIMMSQOL staged the feature is skipped by its tags.
@review @rimmsqol @requires:MalteSchulze.RIMMSqol @requires:nelim.pickletools.rimmsqol @requires:nelim.pickletools.screenshotmode
Feature: RIMMSQOL reveals and hides the Night Change shortcut

  Background:
    Given the save "test-colony" is loaded
    And I close all dialogs
    Then mod "MalteSchulze.RIMMSqol" is loaded
    And RIMMSQOL is ready to be driven

  Scenario: RIMMSQOL's own list offers the shortcut, hidden, and the bar does not draw it
    Then RIMMSQOL's own list of main buttons offers "NightChange_Settings"
    And RIMMSQOL shows the main button "NightChange_Settings" as hidden
    And RIMMSQOL holds no choice for the main button "NightChange_Settings"
    And the main bar does not draw the button "NightChange_Settings"

  Scenario: revealed in RIMMSQOL the shortcut is drawn, and it opens the same page as Mod options
    When RIMMSQOL reveals the main button "NightChange_Settings"
    Then RIMMSQOL shows the main button "NightChange_Settings" as visible
    And RIMMSQOL's settings file records the main button "NightChange_Settings" as visible
    And the main bar draws the button "NightChange_Settings"
    When the main bar's button "NightChange_Settings" is activated
    Then Night Change: the settings dialog is open for this mod
    When Nelim's Pickle Tools: screenshot mode is enabled around the open windows
    And I take a screenshot "night change settings, opened by the shortcut RIMMSQOL revealed"
    And Nelim's Pickle Tools: screenshot mode is disabled
    And I close all dialogs

  Scenario: hidden again in RIMMSQOL the shortcut leaves the bar, and forgetting the choice leaves nothing behind
    Given RIMMSQOL reveals the main button "NightChange_Settings"
    And the main bar draws the button "NightChange_Settings"
    When RIMMSQOL hides the main button "NightChange_Settings"
    Then RIMMSQOL shows the main button "NightChange_Settings" as hidden
    And the main bar does not draw the button "NightChange_Settings"
    When RIMMSQOL forgets its choice for the main button "NightChange_Settings"
    Then RIMMSQOL holds no choice for the main button "NightChange_Settings"
    And RIMMSQOL's settings file records no choice for the main button "NightChange_Settings"
    And no errors were logged

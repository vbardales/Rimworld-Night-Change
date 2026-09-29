# MOD_SETTINGS.md, "Verify the access contract": Mod options is the primary route, and the optional
# MainButtons shortcut is hidden by default (neither visible nor greyed) and opens the same page.
# Played in every pass; in the French pass the captures show the French page.
#
# The values, the clamps and the persistence logic are proved outside the game (13 assertions). What
# only a game shows is that the real Dialog_ModSettings is built for THIS mod by both routes, that the
# shortcut is neither drawn nor greyed on a clean profile, and that revealing it, as a customization
# mod does by moving buttonVisible, puts it in the bar live. RIMMSQOL itself is feature 08.
@review @requires:nelim.pickletools.screenshotmode
Feature: the settings page and its hidden shortcut

  Background:
    Given the save "test-colony" is loaded
    And I close all dialogs
    And Night Change: the settings are at their documented defaults

  Scenario: hidden by default, and it opens Night Change's own page when activated
    Then Night Change: the shortcut is hidden on a clean configuration
    When Night Change: the shortcut is activated
    Then Night Change: the settings dialog is open for this mod
    When Nelim's Pickle Tools: screenshot mode is enabled around the open windows
    And I take a screenshot "night change settings, opened by the shortcut"
    And Nelim's Pickle Tools: screenshot mode is disabled
    And I close all dialogs

  Scenario: Mod options opens the same page
    When Night Change: I open the settings page from Mod options
    Then Night Change: the settings dialog is open for this mod
    When Nelim's Pickle Tools: screenshot mode is enabled around the open windows
    And I take a screenshot "night change settings, opened from mod options"
    And Nelim's Pickle Tools: screenshot mode is disabled
    And I close all dialogs

  Scenario: revealed it is drawn and live, hidden it is gone again
    Then Night Change: the shortcut is not drawn in the bar
    When Night Change: the shortcut is revealed, as a customization mod would
    Then Night Change: the shortcut is drawn in the bar
    When Night Change: the shortcut is hidden again
    Then Night Change: the shortcut is not drawn in the bar
    And no errors were logged

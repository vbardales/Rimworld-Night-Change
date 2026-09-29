# TESTING.md, "translations": every key of the mod resolves in the language of the pass, and the two texts
# a player sees on the stand (the gizmos and the inspect line) come out of those keys. The language is
# fixed at launch (-Language English, then -Language French): nothing here switches it, and no step
# spells a translated word, so the same file runs in both passes.
#
# A capture in developer mode shows a key missing from the active language as accented gibberish; the
# first scenario asks the language itself, so it does not depend on a capture. The captures are what a
# person reads for clipping and wrong paragraph breaks.
@review @requires:nelim.pickletools.screenshotmode
Feature: the text is in the language of the pass

  Scenario: every key of the mod resolves
    Given the save "test-colony" is loaded
    Then Night Change: all 13 keys of the mod resolve in the language of the pass

  Scenario: the stand's gizmos and inspect line, with a borrower
    Given the save "test-colony" is loaded
    And I close all dialogs
    And Night Change: a bedroom is built at x=30 z=30
    And a colonist "Alice" exists
    And I strip "Alice"
    And Night Change: "Alice" is placed in the bedroom
    And Night Change: "Alice" owns the first bed of the bedroom
    And Night Change: an outfit stand is placed in the bedroom
    And Night Change: I hang "Apparel_BasicShirt" on the stand
    And Night Change: "Alice" is dressed in "Apparel_Parka"
    And I set the hour to 23
    And Night Change: "Alice" goes to bed of their own accord
    And Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    Then Night Change: the stand offers the gizmo keyed "NightChange_AssignGizmo"
    And Night Change: the stand offers the gizmo keyed "NightChange_ChangeBackGizmo"
    And Night Change: the stand's inspect line names "Alice"
    When Nelim's Pickle Tools: I select the thing of def "Building_OutfitStand" at (33, 34)
    And Nelim's Pickle Tools: screenshot mode is enabled around the open windows
    And I take a screenshot "night change, the stand with its borrower and its gizmos"
    And Nelim's Pickle Tools: screenshot mode is disabled

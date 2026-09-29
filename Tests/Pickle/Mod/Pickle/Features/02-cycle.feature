# TESTING.md, family "cycle": the evening change, the morning return, and what must survive both.
# The colonist is sent to bed with an ordinary LayDown job, which is the call vanilla's rest giver
# ends in and the one the mod's StartJob prefix hooks. The hour of the day is set, not waited for.
#
# Day clothes: a parka (shell, torso). Night clothes on the stand: a basic shirt and trousers. The
# mod's rule is a FULL change: the parka is parked, not layered over.
#
# The garments are counted before and after: a change that loses or duplicates one is seen whatever
# became of it. Waits are in ticks, in slices, so a paused game is unpaused.
#
# Played in every pass, in both languages (nothing here spells a translated word).
Feature: the evening change and the morning return

  Background:
    Given the save "test-colony" is loaded
    And I close all dialogs
    And Night Change: a bedroom is built at x=30 z=30
    And a colonist "Alice" exists
    And I strip "Alice"
    And Night Change: "Alice" is placed in the bedroom
    And Night Change: "Alice" owns the first bed of the bedroom
    And Night Change: an outfit stand is placed in the bedroom
    And Night Change: I hang "Apparel_BasicShirt" on the stand
    And Night Change: I hang "Apparel_Pants" on the stand
    And Night Change: "Alice" is dressed in "Apparel_Parka"
    And Night Change: I count the garments of the bedroom

  Scenario: going to bed of their own accord, the colonist puts on the night clothes and parks the day clothes
    Given Night Change: the worn "Apparel_Parka" of "Alice" is forced
    And I set the hour to 23
    When Night Change: "Alice" goes to bed of their own accord
    Then Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    And Night Change: "Alice" is wearing "Apparel_Pants"
    And Night Change: "Alice" is not wearing "Apparel_Parka"
    And Night Change: the stand holds "Apparel_Parka"
    And Night Change: the stand does not hold "Apparel_BasicShirt"
    And Night Change: the stand's borrower is "Alice"
    And Night Change: the bedroom holds as many garments as counted
    When I wait for "Alice" to have job "LayDown"
    Then no errors were logged

  Scenario: in the morning the day clothes come back exactly as they were, force-worn markers included
    Given Night Change: the worn "Apparel_Parka" of "Alice" is forced
    And I set the hour to 23
    And Night Change: "Alice" goes to bed of their own accord
    And Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    When Night Change: "Alice" gets up
    And I set the hour to 12
    Then Night Change: "Alice" ends up wearing "Apparel_Parka"
    And Night Change: the worn "Apparel_Parka" of "Alice" is still forced
    And Night Change: "Alice" is not wearing "Apparel_BasicShirt"
    And Night Change: the stand holds "Apparel_BasicShirt"
    And Night Change: the stand holds "Apparel_Pants"
    And Night Change: the stand has no borrower
    And Night Change: the bedroom holds as many garments as counted
    And no errors were logged

  Scenario: day clothes that were not forced come back not forced
    Given I set the hour to 23
    And Night Change: "Alice" goes to bed of their own accord
    And Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    When Night Change: "Alice" gets up
    And I set the hour to 12
    Then Night Change: "Alice" ends up wearing "Apparel_Parka"
    And Night Change: the worn "Apparel_Parka" of "Alice" is not forced

  # The timetable says sleep from late evening to early morning. Getting up for a snack at one in the
  # morning must not trigger three trips to the stand: the night clothes stay on until the sleep
  # hours are over.
  Scenario: a midnight snack does not change the colonist back until the sleep hours are over
    Given I set the hour to 23
    And Night Change: "Alice" goes to bed of their own accord
    And Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    When I set the hour to 1
    And Night Change: "Alice" gets up
    And I wait 600 ticks
    Then Night Change: "Alice" is wearing "Apparel_BasicShirt"
    And Night Change: the stand's borrower is "Alice"
    When I set the hour to 12
    Then Night Change: "Alice" ends up wearing "Apparel_Parka"
    And Night Change: the stand has no borrower

  # "Change back now" interrupts the sleeper so that the return trip starts at once.
  Scenario: the stand's button sends the sleeper back to their own clothes
    Given I set the hour to 23
    And Night Change: "Alice" goes to bed of their own accord
    And Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    And I set the hour to 12
    And Night Change: the stand offers the gizmo keyed "NightChange_ChangeBackGizmo"
    When Nelim's Pickle Tools: I select the thing of def "Building_OutfitStand" at (33, 34)
    And Nelim's Pickle Tools: I click the gizmo keyed "NightChange_ChangeBackGizmo"
    Then Night Change: "Alice" ends up wearing "Apparel_Parka"
    And Night Change: the stand has no borrower

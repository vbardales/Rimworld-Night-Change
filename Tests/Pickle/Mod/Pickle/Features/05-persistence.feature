# TESTING.md, family "persistence": the ledger, the parked clothes and a change in progress survive a
# save and a reload. After a reload every object kept from before belongs to the game that was
# replaced: the steps find the colonist, the stand and the garments again by name and by kind, and the
# garment count is a plain number kept in the scenario.
Feature: the borrowed outfit survives a save

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
    And Night Change: the worn "Apparel_Parka" of "Alice" is forced
    And Night Change: I count the garments of the bedroom
    And I set the hour to 23

  Scenario: saved overnight, the outfit is still borrowed and the morning return still restores it
    Given Night Change: "Alice" goes to bed of their own accord
    And Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    When I save and reload
    Then Night Change: the stand's borrower is "Alice"
    And Night Change: "Alice" is wearing "Apparel_BasicShirt"
    And Night Change: the stand holds "Apparel_Parka"
    And Night Change: the bedroom holds as many garments as counted
    When Night Change: "Alice" gets up
    And I set the hour to 12
    Then Night Change: "Alice" ends up wearing "Apparel_Parka"
    And Night Change: the worn "Apparel_Parka" of "Alice" is still forced
    And Night Change: the stand has no borrower
    And no errors were logged

  # The job is scribed with the garments it is about to move: on load it resumes and finishes, with no
  # garment lost or duplicated.
  Scenario: saved in the middle of the change, the change finishes after the reload
    Given Night Change: "Alice" goes to bed of their own accord
    When I save and reload
    Then Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    And Night Change: the stand's borrower is "Alice"
    And Night Change: the bedroom holds as many garments as counted
    And no errors were logged

# TESTING.md, family "departure": what becomes of the ledger when the stand or the borrower goes away.
# Vanilla returns a destroyed stand's contents to the floor; the mod must empty its ledger, or the
# colonist stays flagged as being in night clothes. Banishment reaches no UnclaimAll in vanilla; the
# mod reaps at the moment of banishment.
Feature: the stand or the borrower goes away

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
    And I set the hour to 23
    And Night Change: "Alice" goes to bed of their own accord
    And Night Change: "Alice" ends up wearing "Apparel_BasicShirt"

  Scenario: the stand is destroyed overnight, nothing loops and no garment is duplicated
    When Night Change: the stand is destroyed
    And Night Change: "Alice" gets up
    And I set the hour to 12
    And I wait 900 ticks
    Then Night Change: the bedroom holds as many garments as counted
    And no errors were logged

  Scenario: the borrower is banished, the stand no longer names them
    When Night Change: "Alice" is banished
    And I wait 300 ticks
    Then Night Change: the stand has no borrower
    And no errors were logged

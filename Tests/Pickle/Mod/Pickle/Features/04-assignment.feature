# TESTING.md, family "assignment": who a stand serves, and how far it may be from the bed.
# An assigned stand serves only its owners. A free stand serves whoever owns a bed in the room
# (setting inheritOwnerFromBed, on by default). In a shared room the first colonist to arrive gets it
# and the other goes to bed dressed. The bedroom's interior is 5 by 5: the near stand is 2.2 cells
# from the first bed, the far corner 4.2.
Feature: which stand serves whom, and from how far

  Background:
    Given the save "test-colony" is loaded
    And I close all dialogs
    And Night Change: a bedroom is built at x=30 z=30
    And a colonist "Alice" exists
    And I strip "Alice"
    And Night Change: "Alice" is placed in the bedroom
    And Night Change: "Alice" owns the first bed of the bedroom
    And Night Change: "Alice" is dressed in "Apparel_Parka"
    And I set the hour to 23

  Scenario: with owner inheritance off an unassigned stand serves nobody
    Given Night Change: an outfit stand is placed in the bedroom
    And Night Change: I hang "Apparel_BasicShirt" on the stand
    And Night Change: I hang "Apparel_Pants" on the stand
    And Night Change: the setting inheritOwnerFromBed is false
    When Night Change: "Alice" goes to bed of their own accord
    And I wait 300 ticks
    Then Night Change: "Alice" is wearing "Apparel_Parka"
    And Night Change: the stand has no borrower

  Scenario: with owner inheritance off a stand assigned to the sleeper still serves them
    Given Night Change: an outfit stand is placed in the bedroom
    And Night Change: I hang "Apparel_BasicShirt" on the stand
    And Night Change: I hang "Apparel_Pants" on the stand
    And Night Change: the setting inheritOwnerFromBed is false
    And Night Change: the stand is assigned to "Alice"
    When Night Change: "Alice" goes to bed of their own accord
    Then Night Change: "Alice" ends up wearing "Apparel_BasicShirt"

  Scenario: a stand assigned to somebody else is not used by the bed's owner
    Given a colonist "Bob" exists
    And I strip "Bob"
    And Night Change: "Bob" is placed in the bedroom
    And Night Change: "Bob" owns a second bed of the bedroom
    And Night Change: an outfit stand is placed in the bedroom
    And Night Change: I hang "Apparel_BasicShirt" on the stand
    And Night Change: I hang "Apparel_Pants" on the stand
    And Night Change: the stand is assigned to "Bob"
    When Night Change: "Alice" goes to bed of their own accord
    And I wait 300 ticks
    Then Night Change: "Alice" is wearing "Apparel_Parka"
    And Night Change: the stand has no borrower
    When Night Change: "Bob" goes to bed of their own accord
    Then Night Change: "Bob" ends up wearing "Apparel_BasicShirt"

  Scenario: in a shared room the first to go to bed gets the stand and the other goes to bed dressed
    Given a colonist "Bob" exists
    And I strip "Bob"
    And Night Change: "Bob" is placed in the bedroom
    And Night Change: "Bob" owns a second bed of the bedroom
    And Night Change: "Bob" is dressed in "Apparel_Duster"
    And Night Change: an outfit stand is placed in the bedroom
    And Night Change: I hang "Apparel_BasicShirt" on the stand
    And Night Change: I hang "Apparel_Pants" on the stand
    When Night Change: "Alice" goes to bed of their own accord
    And Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    And Night Change: "Bob" goes to bed of their own accord
    And I wait 300 ticks
    Then Night Change: the stand's borrower is "Alice"
    And Night Change: "Bob" is wearing "Apparel_Duster"

  Scenario: a stand beyond the maximum distance is not used
    Given Night Change: an outfit stand is placed in the far corner of the bedroom
    And Night Change: I hang "Apparel_BasicShirt" on the stand
    And Night Change: I hang "Apparel_Pants" on the stand
    And Night Change: the setting maxStandDistance is 3
    When Night Change: "Alice" goes to bed of their own accord
    And I wait 300 ticks
    Then Night Change: "Alice" is wearing "Apparel_Parka"
    And Night Change: the stand has no borrower

  Scenario: the same stand within the default distance is used
    Given Night Change: an outfit stand is placed in the far corner of the bedroom
    And Night Change: I hang "Apparel_BasicShirt" on the stand
    And Night Change: I hang "Apparel_Pants" on the stand
    When Night Change: "Alice" goes to bed of their own accord
    Then Night Change: "Alice" ends up wearing "Apparel_BasicShirt"

  # The return trip is built from the ledger, never from the assigned owner: a stand reassigned overnight
  # still gives the sleeper their own clothes back.
  Scenario: a stand reassigned overnight still returns the borrower's own clothes
    Given a colonist "Bob" exists
    And I strip "Bob"
    And Night Change: "Bob" is placed in the bedroom
    And Night Change: an outfit stand is placed in the bedroom
    And Night Change: I hang "Apparel_BasicShirt" on the stand
    And Night Change: I hang "Apparel_Pants" on the stand
    And Night Change: "Alice" goes to bed of their own accord
    And Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    When Night Change: the stand is assigned to "Bob"
    And Night Change: "Alice" gets up
    And I set the hour to 12
    Then Night Change: "Alice" ends up wearing "Apparel_Parka"
    And Night Change: the stand has no borrower

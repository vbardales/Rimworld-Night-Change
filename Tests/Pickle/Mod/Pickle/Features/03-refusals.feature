# TESTING.md, family "refusals": what the mod deliberately does not do. Each scenario ends with the day
# clothes still on the colonist and nobody named as the stand's borrower.
#
# The cold guard's boundary values (exactly at the threshold, margin changes, no insulation loss) are
# proved outside the game (Tests/Program.cs, 13 assertions). What only a game shows is that the real
# room temperature, the real insulation stats and the real StartJob hook agree with that decision.
#
# The raider scenarios lean on the danger watcher, which updates on its own cadence; they wait ticks
# and assert the danger first, so a fixture that never produced danger fails on that line and not on
# the mod. They are the least stable scenarios of the suite.
Feature: what the mod refuses to do

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
    And I set the hour to 23

  Scenario: a direct order to lie down is never delayed by a wardrobe trip
    When Night Change: "Alice" is ordered to bed by the player
    Then "Alice" has job "LayDown"
    When I wait 300 ticks
    Then Night Change: "Alice" is wearing "Apparel_Parka"
    And Night Change: the stand has no borrower

  Scenario: medical rest is left alone
    Given Night Change: "Alice" has a wound that calls for medical rest
    When Night Change: "Alice" goes to bed of their own accord
    And I wait 300 ticks
    Then Night Change: "Alice" is wearing "Apparel_Parka"
    And Night Change: the stand has no borrower

  Scenario: no changing during a raid
    Given Night Change: a raider stands at the edge of the colony
    And I wait 300 ticks
    And Night Change: the map reports danger
    When Night Change: "Alice" goes to bed of their own accord
    And I wait 600 ticks
    Then Night Change: "Alice" is wearing "Apparel_Parka"
    And Night Change: the stand has no borrower

  # The gate covers the outbound trip only: a colonist leaving their night clothes is walking toward
  # their own gear, and the return must stay possible whatever the danger.
  Scenario: the return trip stays allowed during a raid
    Given Night Change: "Alice" goes to bed of their own accord
    And Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    And Night Change: a raider stands at the edge of the colony
    And I wait 300 ticks
    And Night Change: the map reports danger
    When Night Change: "Alice" gets up
    And I set the hour to 12
    Then Night Change: "Alice" ends up wearing "Apparel_Parka"

  # A parka to a shirt and trousers is a large loss of insulation: in a room at minus thirty the guard
  # refuses, in a warm room it accepts, and switched off it accepts in the cold too.
  Scenario: the cold guard refuses the change when the bedroom is too cold
    Given Night Change: the bedroom is at -30 degrees
    When Night Change: "Alice" goes to bed of their own accord
    And I wait 300 ticks
    Then Night Change: "Alice" is wearing "Apparel_Parka"
    And Night Change: the stand has no borrower

  Scenario: the cold guard accepts the change in a warm bedroom
    Given Night Change: the bedroom is at 21 degrees
    When Night Change: "Alice" goes to bed of their own accord
    Then Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    And Night Change: the stand's borrower is "Alice"

  Scenario: with the cold guard off the change happens in a cold bedroom
    Given Night Change: the setting coldGuard is false
    And Night Change: the bedroom is at -30 degrees
    When Night Change: "Alice" goes to bed of their own accord
    Then Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    And Night Change: the stand's borrower is "Alice"

  # A drafted colonist is not eligible: nobody changes into night clothes with a weapon raised.
  Scenario: a drafted colonist is not sent to the stand
    Given I draft "Alice"
    When Night Change: "Alice" goes to bed of their own accord
    And I wait 300 ticks
    Then Night Change: "Alice" is wearing "Apparel_Parka"
    And Night Change: the stand has no borrower

  # A fire on the stand: the swap job carries FailOnBurningImmobile on its target, and the colonist
  # must not be left stuck in the burning room's wardrobe trip. Added 2026-10-16 (TESTING.md, "fire:
  # not exercised"). The stand may burn down within the wait; nothing here reads the stand again.
  Scenario: a burning stand does not make the colonist change
    Given Night Change: a fire starts on the stand
    When Night Change: "Alice" goes to bed of their own accord
    And I wait 300 ticks
    Then Night Change: "Alice" is not wearing "Apparel_BasicShirt"
    And no errors were logged

  # Eligible() refuses a downed pawn: carried to bed by someone else, they are not sent to a stand.
  Scenario: a downed colonist is put to bed in their own clothes
    Given Night Change: "Alice" is downed
    When Night Change: "Alice" goes to bed of their own accord
    And I wait 300 ticks
    Then Night Change: "Alice" is wearing "Apparel_Parka"
    And Night Change: the stand has no borrower
    And no errors were logged

  # Downed overnight: the return trip refuses a downed pawn (nobody walks to a stand crawling), the
  # ledger stays honest, and the return happens once they can walk again.
  Scenario: downed in night clothes, the return waits until they are on their feet
    Given Night Change: "Alice" goes to bed of their own accord
    And Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    And Night Change: "Alice" is downed
    When Night Change: "Alice" gets up
    And I set the hour to 12
    And I wait 600 ticks
    Then Night Change: "Alice" is wearing "Apparel_BasicShirt"
    And Night Change: the stand's borrower is "Alice"
    When Night Change: "Alice" is healed
    Then Night Change: "Alice" ends up wearing "Apparel_Parka"
    And no errors were logged

  # A stand the colonist cannot reach is no stand: the outbound change is not chosen, and the
  # deferred bed job is not lost.
  Scenario: a blocked path to the stand leaves the colonist in their own clothes
    Given Night Change: the stand's way is walled up
    When Night Change: "Alice" goes to bed of their own accord
    And I wait 600 ticks
    Then Night Change: "Alice" is wearing "Apparel_Parka"
    And Night Change: the stand has no borrower
    And no errors were logged

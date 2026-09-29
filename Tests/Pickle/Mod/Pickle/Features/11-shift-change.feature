# Coexistence with Shift Change (MrBeverage.ShiftChange, Workshop 3783456242): both mods put a
# CompAssignableToPawn on the SAME outfit stand def. Night Change scribes with its own prefixed keys and
# strips the gizmo's hotkey so that the two comps do not cross-read each other on load or collide on a key.
# Loads after Shift Change (loadAfter). Played only by the pass "avec-shiftchange".
#
# What is asserted: the stand carries both comps, the evening change still works beside them, a save and
# a reload keep the borrowed outfit, and nothing logs an error. What is NOT: Shift Change's own behavior.
@requires:MrBeverage.ShiftChange
Feature: Night Change beside Shift Change

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

  Scenario: both mods' comps are on the stand and the evening change still works
    Then mod "nelim.nightchange" loads after "MrBeverage.ShiftChange"
    And Night Change: the stand carries 2 assignable comps
    When Night Change: "Alice" goes to bed of their own accord
    Then Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    And Night Change: the stand's borrower is "Alice"
    And no errors were logged

  Scenario: the borrowed outfit survives a save beside the other comp
    Given Night Change: "Alice" goes to bed of their own accord
    And Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    When I save and reload
    Then Night Change: the stand's borrower is "Alice"
    And Night Change: the stand carries 2 assignable comps
    And no errors were logged

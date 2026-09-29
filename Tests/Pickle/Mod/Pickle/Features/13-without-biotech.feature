# The mod's own guard: without Biotech there is no kid outfit stand, and the conditional patch must
# neither throw nor log a missing-target error, and the ordinary stand still works. What is NOT tested
# is the game's reaction to the missing DLC: the pass leaves Biotech out (`!ludeon.rimworld.biotech` in
# its map) and this feature asserts what the MOD does.
#
# Played only by the pass "sans-biotech" (tag @sans-biotech, excluded from the other passes).
@sans-biotech
Feature: Night Change without Biotech

  Scenario: no kid stand, no error, and the ordinary stand works
    Given the save "test-colony" is loaded
    Then mod "Ludeon.RimWorld.Biotech" is not loaded
    And no def "Building_KidOutfitStand" exists
    And def "Building_OutfitStand" was patched by mod "Night Change"
    And Night Change: the mod has not disabled itself
    And no errors were logged

  Scenario: the evening change works without Biotech
    Given Night Change: a bedroom is built at x=30 z=30
    And a colonist "Alice" exists
    And I strip "Alice"
    And Night Change: "Alice" is placed in the bedroom
    And Night Change: "Alice" owns the first bed of the bedroom
    And Night Change: an outfit stand is placed in the bedroom
    And Night Change: I hang "Apparel_BasicShirt" on the stand
    And Night Change: I hang "Apparel_Pants" on the stand
    And Night Change: "Alice" is dressed in "Apparel_Parka"
    And I set the hour to 23
    When Night Change: "Alice" goes to bed of their own accord
    Then Night Change: "Alice" ends up wearing "Apparel_BasicShirt"
    And no errors were logged

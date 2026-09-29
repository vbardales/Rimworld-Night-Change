# TESTING.md, family "loads". What only the game's real patch pipeline shows: the def the mod patches
# carries the mod's comp, the think tree and job are registered, and nothing threw at load. The mod's
# own FailOpen switch is read too: a hook that threw once disables the mod for the session, quietly.
#
# Played in every pass. No capture.
Feature: Night Change loads and patches the outfit stand

  Scenario: the mod, its defs and its patch are in the game
    Given the save "test-colony" is loaded
    Then mod "nelim.nightchange" is loaded
    And mod "nelim.nightchange" loads after "brrainz.harmony"
    And mod "nelim.nightchange" loads after "Ludeon.RimWorld.Odyssey"
    And def "NightChange_ChangeAtStand" exists
    And def "NightChange_ChangeBack" exists
    And def "NightChange_Settings" of type "MainButtonDef" exists
    And def "Building_OutfitStand" was patched by mod "nelim.nightchange"
    And Night Change: the mod has not disabled itself
    And no errors were logged

# The conditional patch on Biotech's kid outfit stand. Played when Biotech is loaded (the default staging
# has every DLC). The pass without Biotech is feature 13.
@requires:ludeon.rimworld.biotech
Feature: the kid outfit stand carries the comp when Biotech is present

  Scenario: the patch reaches the kid outfit stand
    Given the save "test-colony" is loaded
    Then def "Building_KidOutfitStand" was patched by mod "nelim.nightchange"
    And no errors were logged

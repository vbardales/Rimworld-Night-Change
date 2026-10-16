# TESTING.md, family "gallery": the five captures of PUBLICATION.md, section 2.
# Each scenario is one image (GALLERY.md). Captures carry @review: the scenario passing proves
# only that the scene was set up, the image is opened and looked at by the session (shootGallery 9.d).
#
# Cast: Nelim (short hair, neutral tired face) and Alice (background, long hair, faint smile).
# Vanilla pawns only, no optional mod. Hour set, not waited for. Captures are cropped to the
# bedroom and the stand, no interface, except the two interface captures (stand gizmos, settings).
#
# Played in the minimal pass (wsl-deps.sans-facultatifs.map) only.
Feature: the gallery captures of the evening, the night and the morning

  Background:
    Given the save "test-colony" is loaded
    And I close all dialogs
    And Night Change: a bedroom is built at x=30 z=30
    And a colonist "Nelim" exists
    And a colonist "Alice" exists
    And I strip "Nelim"
    And I strip "Alice"
    And Nelim's Pickle Tools: "Nelim" hairstyle is "Bob"
    And Nelim's Pickle Tools: "Nelim" hair colour is rgb (70, 52, 38)
    And Nelim's Pickle Tools: "Alice" hairstyle is "Long"
    And Nelim's Pickle Tools: "Alice" hair colour is rgb (176, 120, 70)
    And Night Change: "Nelim" is placed in the bedroom
    And Night Change: "Nelim" owns the first bed of the bedroom
    And Night Change: "Alice" is placed in the bedroom
    And Night Change: an outfit stand is placed in the bedroom
    And Night Change: I hang "Apparel_BasicShirt" on the stand
    And Night Change: I hang "Apparel_Pants" on the stand
    And Night Change: "Nelim" is dressed in "Apparel_Parka"
    And Nelim's Pickle Tools: the roof is removed from the area from (27, 27) to (34, 34)

  @review @gallery
  Scenario: 1-evening-change, Nelim takes off the parka and hangs it on the stand
    Given Nelim's Pickle Tools: "Nelim" facial expression is "normal"
    And Nelim's Pickle Tools: "Nelim" face is held neutral
    And Nelim's Pickle Tools: "Nelim" mouth is "MouthLipsSmallSmile"
    And I set the hour to 22
    And Night Change: the worn "Apparel_Parka" of "Nelim" is forced
    When Night Change: "Nelim" goes to bed of their own accord
    And I wait for "Nelim" to have job "LayDown"
    And Night Change: "Nelim" ends up wearing "Apparel_BasicShirt"
    And I take a screenshot "1-evening-change"

  @review @gallery
  Scenario: 2-overnight, Nelim asleep, day clothes parked on the stand
    Given Night Change: "Nelim" goes to bed of their own accord
    And Night Change: "Nelim" ends up wearing "Apparel_BasicShirt"
    And I set the hour to 2
    And I wait for "Nelim" to have job "LayDown"
    And Nelim's Pickle Tools: "Nelim" facial expression is "blink"
    And Nelim's Pickle Tools: "Nelim" face is held neutral
    And I take a screenshot "2-overnight"

  @review @gallery @interface
  Scenario: 3-stand-gizmos, the Set sleeper and Change back now buttons on the stand
    Given Night Change: "Nelim" goes to bed of their own accord
    And Nelim's Pickle Tools: I select the thing of def "OutfitStand" at (31, 31)
    When Nelim's Pickle Tools: screenshot mode is enabled around the open windows
    And I take a screenshot "3-stand-gizmos"
    And Nelim's Pickle Tools: screenshot mode is disabled

  @review @gallery @interface
  Scenario: 4-settings, the Night Change window from Mod options
    When Nelim's Pickle Tools: screenshot mode is enabled around the open windows
    And Night Change: I open the settings page from Mod options
    And I take a screenshot "4-settings"
    And Nelim's Pickle Tools: screenshot mode is disabled

  @review @gallery
  Scenario: 5-morning-change, Nelim wakes and puts the parka back on
    Given Night Change: "Nelim" goes to bed of their own accord
    And Night Change: "Nelim" ends up wearing "Apparel_BasicShirt"
    And I set the hour to 6
    When Night Change: "Nelim" gets up
    Then Night Change: "Nelim" ends up wearing "Apparel_Parka"
    And Nelim's Pickle Tools: "Nelim" facial expression is "SocialRelax"
    And I take a screenshot "5-morning-change"

# PandemicLegacyHelper

A small fan-made tool made to track the infection cards for the cooperative boardgame Pandemic Legacy season 2. In this game players work together to save humanity from a deadly plague by being suppliers and travelling the world, with a campaign that unfolds over multiple sessions, altering the game board and components permanently.

This tool has been made in [Unity](https://unity.com/) and meant to be used on a mobile phone.

## Guide

This app can be dowloaded from this repository by downloading one of the files in `/Build` as APK or ... . A playable version on Itch.io can be accessed here ...

### Draw Piles

This tool tracks the infection cards draw pile and open pile, especially for cards that get added to the top of the drawpile after a Pandemic card has been drawn. The app splits this into 3 different piles that are sorted on probability of occuring:
- `Draw Pile`:
Holds all the infection cards from the start of the game or new month.
- `Known Draw Pile`:
    This pile represents the top of the `Draw pile` which are the cards that have been in the `Open pile` and moved back because of the Pandemic event. This deck can have different subpiles.
- `Open Pile`:
Holds all cards that have been drawn already.

### Cards
Cards are presented as a grouping of the same name making it easier to see whether cards have a higher change to appear. However cards will still be moved individually.

![Card Example](https://github.com/LuckyJollyJumper/PandemicLegacyHelper/blob/main/ScreenShots/CardExample.png?raw=true)

### Drawing cards
Cards can be drawn from the `Draw Pile` and the `Known Draw Pile` by clicking on the button with ">". Which moves a single card from that group to the `Open Pile`.

Cards in the `Open Pile` can be removed by using the "X" button and will remove a single card from the grouping.

![Card Example](https://github.com/LuckyJollyJumper/PandemicLegacyHelper/blob/main/ScreenShots/CardExample2.png?raw=true)

### Adding cards
To add cards to the tool the button Add Infection Card can be used to add a single grouping of a card to the `Draw Pile`. DUPLICATE NAMES WILL BE GROUPED TOGETHER!

![Add Card Button](https://github.com/LuckyJollyJumper/PandemicLegacyHelper/blob/main/ScreenShots/AddCard.png?raw=true)

The Pandemic button will move all the cards in the `Open Pile` to a new subset in the `Known Draw Pile`.

![Pandemic Button](https://github.com/LuckyJollyJumper/PandemicLegacyHelper/blob/main/ScreenShots/PandemicButton.png?raw=true)

### Resetting/saving the game
The game saves all your cards whenever a change is done in one of the decks.

Resetting for the new month can be accessed via the settings button (Cog in the upper right). This will return all active cards in all decks back to the `Draw Pile`. The settings also have a button to reset all cards to the infection cards present at the start of the game.

## Contributing

If you want to help or have any ideas to extend this application for more of the games, you can do so by reporting a bug or suggesting new features. Anything is welcome!

If you like to write something yourself, or can show me how to do things better, pull requests are also very much appreciated. I am always open to learn new things.

## Disclaimer
The tool and all content on this GitHub page are provided "as is" without warranties of any kind, express or implied. This project is an unofficial, fan-made tool created for personal and community use to assist with tracking card states in the board game Pandemic Legacy: Season 2. It is not endorsed by, affiliated with, or sponsored by the game’s publisher, designers, or rights holders. All original code and content in this repository are released under the project’s chosen license (see LICENSE).

If you believe this repository infringes your intellectual property rights or contains content that should be removed, contact the repository owner directly to resolve the issue.

Last updated: March 29, 2026

## TODO
- Fix PopUp Dropdown sizes
- Update UI
- Add special stickers to cards


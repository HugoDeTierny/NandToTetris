# Number Guessing Game - Jack Programming Language

## Project Overview
This is an interactive Number Guessing Game implemented in the Jack programming language as part of Project 9 of the Nand2Tetris course. The game demonstrates various programming concepts including object-oriented design, user interaction, random number generation, and game state management.

## Features

### Core Gameplay
- **Interactive Guessing**: Players try to guess a randomly generated number within a specified range
- **Multiple Difficulty Levels**: 
  - Easy: Numbers 1-50, 10 attempts
  - Medium: Numbers 1-100, 8 attempts  
  - Hard: Numbers 1-200, 6 attempts
- **Smart Hints**: The game provides feedback on whether guesses are too high, too low, and how close they are
- **Attempt Tracking**: Shows current attempt number and remaining attempts

### Scoring System
- **Base Score**: 100 points for successfully guessing the number
- **Attempt Bonus**: 10 extra points for each unused attempt
- **Difficulty Multiplier**: Easy (x1), Medium (x2), Hard (x3)
- **Score Persistence**: Total score and statistics tracked across games

### User Interface
- **Menu System**: Clean, intuitive menu navigation
- **Statistics Tracking**: View games played, total score, and average score
- **Instructions**: Built-in help system explaining rules and scoring
- **Dynamic Feedback**: Real-time hints and encouragement

## Technical Implementation

### Classes and Architecture
1. **Main.jack**: Entry point and application launcher
2. **GuessingGame.jack**: Core game logic, UI, and state management
3. **RandomGenerator.jack**: Pseudo-random number generation using Linear Congruential Generator

### Programming Techniques Demonstrated
- **Object-Oriented Design**: Multiple classes with clear responsibilities
- **State Management**: Game state, player statistics, and settings persistence
- **Algorithm Implementation**: Custom random number generator with LCG algorithm
- **User Interface Design**: Menu systems, formatted output, input validation
- **Mathematical Operations**: Score calculations, range operations, absolute values
- **Control Flow**: Complex nested loops, conditional logic, boolean operations
- **Memory Management**: Proper object construction and disposal

### Random Number Generation
The game implements a sophisticated pseudo-random number generator using the Linear Congruential Generator (LCG) algorithm with carefully chosen parameters:
- Multiplier (a): 1664525
- Increment (c): 1013904223  
- Modulus (m): 32767

This ensures good distribution of random numbers across the specified ranges.

## How to Play

1. **Start the Game**: Run the compiled Jack program
2. **Choose Options**: Navigate the main menu to:
   - Play a game
   - Change difficulty level
   - View your statistics
   - Read instructions
   - Quit the game
3. **Make Guesses**: Enter numbers within the specified range
4. **Use Hints**: Pay attention to "too high/low" and proximity hints
5. **Score Points**: Try to guess in fewer attempts for higher scores

## Educational Value

This project demonstrates mastery of:
- **Jack Language Features**: Classes, methods, fields, constructors, arrays
- **Programming Concepts**: Algorithms, data structures, user interaction
- **Software Design**: Modular design, separation of concerns, code organization
- **Problem Solving**: Random number generation, scoring algorithms, user experience design

## Code Quality Features
- **Comprehensive Documentation**: Every class and method documented
- **Readable Code**: Clear variable names, logical structure, proper indentation
- **Error Handling**: Input validation and graceful error recovery
- **Extensible Design**: Easy to add new features or modify existing ones

## Files Included
- `source/Main.jack` - Application entry point
- `source/GuessingGame.jack` - Main game logic and UI
- `source/RandomGenerator.jack` - Random number generation
- `README.md` - This documentation file

This project aims for the "Very Good" to "Excellent" grade range by providing an engaging user experience with clever programming techniques and ingenious use of the Jack language capabilities.

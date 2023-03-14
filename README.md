# sudoku-puerts
This is a branch of sudoku with PuerTS support. You must install node.js before you open this project.

Some C# scripts has been reimplemented in TypeScript, e.g. `Sudoku.Gameplay.Puzzle.Sudoku9x9`.

## How to setup
1. Install node.js and Unity 2018.4.36f1 first.
2. run !setup_puerts_env.bat, this will setup the `Puer-Project` (mostly `npm init -y` and `npm install`).
3. Profit, you're done setting up the repo. Your TypeScript sources are located in `./Puer-Project/src/`. Now you're free to open the project in Unity.

## Useful scripts
There are a few handful scripts located in the root folder of this repository.

Most scripts requires you to install node.js and Unity 2018.4.36f1 first.

- `!setup_puerts_env.bat`: Will setup the PuerTS environment automatically.
- `!build_puerts_scripts.bat`: Will build TypeScript sources (located in `./Puer-Project/src`) and copy output JavaScript files to the correct location (`./Assets/Resources/TypeScript`).
- `!watch_puerts_scripts.bat`: Will watch TypeScript sources and build them automatically when they are changed.
- `!generate_puerts_wrappers.bat`: Will call Unity Editor to generate PuerTS wrappers. Output files are located in `./Assets/Gen`.
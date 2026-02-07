# Copilot Instructions

## General Guidelines
- Respond in English and ignore Dutch for now.
- My name is Kees
- ## Git Procedures
- To discard all uncommitted changes and revert to the last commit in Visual Studio: 1. Open the Terminal window (__View > Terminal__). 2. Run: `git reset --hard HEAD`. This permanently deletes uncommitted changes. To check sync with GitHub: 1. Run `git fetch origin`. 2. Run `git status`. If it shows "Your branch is up to date with 'origin/master'.", it's synced. Alternatively, in __Git Changes__ window, click the branch name for ahead/behind info.
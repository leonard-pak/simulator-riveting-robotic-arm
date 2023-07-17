# Unity + Git + Git LFS

This is a tutorial & template for setting up Unity + Git + Git Large File Storage (LFS). 

There are a couple of challenges when it comes to using git (or version control software in general) with Unity:

- Binary assets - commit history, merge conflicts, etc. are near impossible for a human to read. We can solve this by serializing assets, e.g. turning Unity scenes into a text format.
- Large files (large 3D models, big spritesheets, etc.) - GitHub warns you at 50mb and caps you at 100mb. We can solve this with [Git LFS](https://git-lfs.github.com/).

## Initial Setup

The initial setup for a project requires a few steps, but you only have to do it once per project. To set up a Unity project with Git and Git LFS:

### Preparing

- Install the required software on your machine:
  - [Unity](https://unity.com/).
  - [Git](https://git-scm.com/).
  - [Git LFS](https://git-lfs.com).

### Creating the Repository

1. Create a new repository for your project from GitHub:
    1. Create from this template: `Use this template > Create a new repository`
    2. Give it a name.
    3. Update README.md after creating.
2. Add your Unity project to the repository:
    1. Create a new Unity project or grab an existing project.
    2. Check `Project Settings` in Unity project:
       - `Version Control / Mode: “Visible Meta Files”`
       - `Asset Serialization / Mode: “Force Text”`
    4. Open up your Unity project folder in your command prompt/terminal.
    5. Create a local git repository and connect it to a previosly created repository from Github
        ``` bash
        git init
        git branch -M main # if local default branch is not main
        git remote add origin git@github.com:youruser/yourrepo.git
        git fetch origin
        git reset --hard origin/master
        git fetch git@github.com:youruser/yourrepo.git main:origin/main
        git checkout main 
        ```
    It is important that the name of the branch in the local repository and the name of the branch in the repository from GitHub are the same
3. Set up Git LFS (only once):
    - Navigate to your repository and run `git lfs install` in your command prompt/terminal. You should see a message like "Git LFS initialized".
4. Push your project:
    1. Add all files: `git add .`
    2. Make commit: `git commit -m 'init commit'`
    3. First push: `git push --set-upstream origin main`

Now that the repository is all set up, you can simply commit and push/pull as you work on the project! Make sure to always commit and sync before leaving the lab.

## Notes

One thing to note, the free plan of Git LFS comes with 1g of storage total and 1g of bandwidth per billing cycle/month. Anything you push/pull from LFS counts against that. You can see your current usage under `Settings > Billing`.

## Resources

- [Version control: Effective use, issues and thoughts, from a gamedev perspective](https://www.what-could-possibly-go-wrong.com/version-control/) - a good overview of the uses and challenges of version control in game dev
- [How to Git with Unity](https://thoughtbot.com/blog/how-to-git-with-unity) - a concise post of Git + Unity
- [Unity's manual of VCS](https://docs.unity3d.com/Manual/VersionControl.html)

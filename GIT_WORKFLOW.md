# Git Workflow Guide for ECommerce Repository

This guide explains how to commit and push changes in the ECommerce repository, with a specific focus on frontend changes.

## Current Repository Structure

```
ECommerce/
├── .git/
├── .gitignore
├── README.md
└── backend/
```

**Note**: Currently, there is no `frontend/` directory in the repository. This guide covers how to work with it once it's created.

## Branch Information

- **Current Branch**: `copilot/commit-and-push-frontend-changes`
- **Remote**: `origin/copilot/commit-and-push-frontend-changes`

## Standard Git Workflow for Frontend Changes

### Step 1: Check Current Status

Before making any changes, always check the current status of your repository:

```bash
git status
```

This will show you:
- Which branch you're on
- Which files have been modified
- Which files are staged for commit
- Which files are untracked

### Step 2: Add Frontend Changes to Staging Area

To add all changes in the frontend directory:

```bash
git add frontend/
```

To add specific files:

```bash
git add frontend/src/components/MyComponent.tsx
git add frontend/package.json
```

To add all changes in the entire repository:

```bash
git add .
```

### Step 3: Verify Staged Changes

Before committing, review what you're about to commit:

```bash
git status
git diff --staged
```

The first command shows which files are staged, and the second shows the actual changes.

### Step 4: Commit Changes

Create a commit with a descriptive message:

```bash
git commit -m "Add new frontend component for user authentication"
```

For multi-line commit messages:

```bash
git commit -m "Add user authentication frontend component

- Created login form component
- Added validation logic
- Integrated with backend API
- Added unit tests"
```

### Step 5: Push Changes to Remote Repository

Push your commits to the remote branch:

```bash
git push origin copilot/commit-and-push-frontend-changes
```

Or if you're on the correct branch already:

```bash
git push
```

## Complete Command Sequence Example

Here's a complete example of committing and pushing frontend changes:

```bash
# 1. Check current status
git status

# 2. Add frontend changes
git add frontend/

# 3. Verify what's being committed
git status
git diff --staged

# 4. Commit with a message
git commit -m "Update frontend components and styling"

# 5. Push to remote
git push origin copilot/commit-and-push-frontend-changes
```

## Alternative Workflow: Automated Reporting (For CI/CD Systems)

In automated environments or when using specialized tools, you may use the `report_progress` tool instead of manual git commands. This tool automatically handles:
- Staging all changes (`git add .`)
- Creating a commit with the provided message
- Pushing changes to the remote branch

## Best Practices

1. **Commit Often**: Make small, focused commits rather than large ones
2. **Write Clear Messages**: Use descriptive commit messages that explain what and why
3. **Review Before Committing**: Always use `git status` and `git diff` before committing
4. **Pull Before Push**: If working with others, pull latest changes first:
   ```bash
   git pull origin copilot/commit-and-push-frontend-changes
   ```
5. **Use .gitignore**: Ensure build artifacts and dependencies are properly ignored
   - Current .gitignore excludes: `.vs/`, `bin/`, `obj/`, `*.user`, `*.suo`
   - For frontend, you may want to add: `node_modules/`, `dist/`, `.env`

## Common Scenarios

### Scenario 1: New Frontend Files

If you create new files in a frontend directory:

```bash
git add frontend/
git commit -m "Add new frontend files for product catalog"
git push
```

### Scenario 2: Modified Frontend Files

If you modify existing frontend files:

```bash
git add frontend/
git commit -m "Fix styling issues in product card component"
git push
```

### Scenario 3: Deleted Frontend Files

If you delete frontend files:

```bash
git add frontend/
git commit -m "Remove deprecated frontend components"
git push
```

The `git add` command will stage the deletions as well.

### Scenario 4: Mixed Changes (Add, Modify, Delete)

For all types of changes in the frontend directory:

```bash
git add frontend/
git commit -m "Refactor frontend architecture

- Removed old components
- Added new Redux store
- Updated routing configuration"
git push
```

## Troubleshooting

### Issue: "Nothing to commit, working tree clean"

This means there are no changes to commit. Verify you've made changes:

```bash
git status
ls -la frontend/
```

### Issue: "Updates were rejected"

This usually means the remote has changes you don't have locally. Pull first:

```bash
git pull origin copilot/commit-and-push-frontend-changes
# Resolve any conflicts if they occur
git push origin copilot/commit-and-push-frontend-changes
```

### Issue: "Frontend directory doesn't exist"

Create the frontend directory first:

```bash
mkdir -p frontend
# Add your frontend files
git add frontend/
git commit -m "Initialize frontend structure"
git push
```

## GitIgnore Configuration

The current `.gitignore` file excludes:
- `.vs/` - Visual Studio settings
- `bin/` - Binary output
- `obj/` - Object files
- `*.user` - User-specific files
- `*.suo` - Visual Studio solution user options

For frontend development, consider adding:
```
# Frontend
node_modules/
dist/
build/
.env
.env.local
*.log
.DS_Store
```

## Quick Reference

| Action | Command |
|--------|---------|
| Check status | `git status` |
| Add all frontend changes | `git add frontend/` |
| Add specific file | `git add frontend/path/to/file` |
| Add all changes | `git add .` |
| Commit changes | `git commit -m "message"` |
| Push to remote | `git push` |
| Pull from remote | `git pull` |
| View commit history | `git log --oneline` |
| View changes | `git diff` |
| View staged changes | `git diff --staged` |

## Summary

The essential command sequence for committing and pushing frontend changes:

```bash
git add frontend/
git commit -m "Your descriptive commit message"
git push
```

Remember to always review your changes before committing and write clear, descriptive commit messages that explain what you've changed and why.

# Contributing
### Reporting Issues
Please only use the issue tracker for reporting bugs. A missing feature or feature request is not a bug. For requests, please visit the Trello Storyboard to see what's in the pipeline, or comment on the community forum to make a request.

When reporting bugs, please use the following template:
```
SUMMARY
<A short abstract on what happened>

EXPECTED BEHAVIOR
<The expected behavior of the system with the bug>

ACTUAL BEHAVIOR
<How the behavior differs from the expected behavior>

REPEAT STEPS
1. <First step, usually logging into server using the client>
2. <Additional steps on repeating the bug>
```

### Forking Comet
If you'd like to take Comet in a new direction or perform a lot of work, then I highly recommend forking the project. You can fork Comet using the "Fork" button on the front page of the GitLab repository. This copies the project repository and all commits to your own GitLab profile. If you choose to make your fork public, it also allows others to find your modified version of Comet more easily through GitLab.

### Merge Requests
After creating a merge request from a fork, the request will be reviewed for quality and consistency. While your request is open, you may receive comments before being allowed to merge the code. Here are some things to keep in mind when creating a merge request:

- Database changes result in a new migration script labled as (yyyymmdd-commit).
- Database changes result in an updated initial deployment script.
- All new code is commented and follows the style and conventions of the project.
- Code is performant and will not cause bottlenecks or deadlocks.

Happy coding!
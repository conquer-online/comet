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

### Programming for Comet
Thank you for your interest in contributing to Comet. All merge requests are reviewed for quality and consistency. If your request is denied, check the comments and see what corrections are recommended. Some things to keep in mind:

- Database changes result in a new migration script labled as (yyyymmdd-commit).
- Database changes result in an updated initial deployment script.
- All new code is commented and follows the style and conventions of the project.
- Code is performant and will not cause bottlenecks or deadlocks.

Happy coding!
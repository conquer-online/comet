# Contribution Guidelines
When contributing to this repository, please first discuss the change you wish to make via an issue, email, or any other method with the owners of this repository before making the change. Please note developers expect to follow a code of conduct for all interactions with the project.

## Reporting Issues
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

## Forking
If you'd like to take the project in a new direction or perform a section of work, then I highly recommend forking the project. You can fork the repository using the "Fork" button on the front page of the GitLab repository. This copies the project repository and all commits to your own GitLab profile. If you choose to make your fork public, it also allows others to find your modified version of the project more easily through GitLab.

## Merge Requests
Code submitted for merging should follow general style guidelines for the language used by the project. Commit summary names should also follow general practices. Writing descriptive commit messages keeps the repository well managed and makes it easy to navigate through the commit log. Your commit message should be short, in present tense and explicitly say why you made the change.

In terms of the procedure for creating merge requests:

1. Ensure that the project builds and the client can log in
2. Update the readme with any changes, if necessary
3. Update public API documentation with any changes, if necessary
4. Code is documented and addresses a functional or behavioral requirement
5. Commit your changes to its own fork and create a merge request in GitLab

## Inclusive Terminology
As recommended by the Engineering Council, and supported by the contributors of this repository, Comet uses naming guidelines which support diversity and inclusivity. When developing with Comet, please take note of the following alternatives to frequently used terms.

Numerous technical organizations have identified terminology they find problematic in their code, standards and documentation and have taken the initiative to start using alternative terms.  Here are some of the most common terms that have been identified by other organizations as problematic and some of the alternatives they have suggested:

| Term                                  | Alternative                                       | References                            |
|:--------------------------------------|:--------------------------------------------------|----------------------------------------
| Whitelist                             | Allowlist, approved list, pass list, accept list, permitted    |[Bluetooth SIG](https://btprodspecificationrefs.blob.core.windows.net/language-mapping/Appropriate_Language_Mapping_Table.pdf?mkt_tok=eyJpIjoiTWpOa05EUmxaV1EwWkRWbCIsInQiOiJnR1dHMEdpeWNTcXFVdG92NVhGNkVPcEdmWGFlOWppbmMzNVBJdkZiTUxXRjk5bG5cL3RHV3JaSDBXRVhkMThoMEVFRVZ3NXlzOUYyekl4TmJmVlM4bHJZa0x4YmowdFJjOTBMT2d4b2l4eEMxcmIya0FcL1IrSk5KVGw2OVZGV1wvXC8ifQ%3D%3D), [3GPP](https://datatracker.ietf.org/liaison/1716/),  [W3C](https://w3c.github.io/manual-of-style/#inclusive), [Avnu](https://avnu.org/wp-content/uploads/2014/05/Avnu_Open-Letter_Inclusive-Terminology-and-Language_July-14-2020_Final.pdf), [Linux](https://git.kernel.org/pub/scm/linux/kernel/git/torvalds/linux.git/commit/?id=49decddd39e5f6132ccd7d9fdc3d7c470b0061bb), [Ansible](https://www.redhat.com/en/blog/making-open-source-more-inclusive-eradicating-problematic-language), [Twitter](https://twitter.com/TwitterEng/status/1278733305190342656), [Chromium](https://chromium.googlesource.com/chromium/src/+/master/styleguide/inclusive_code.md), [Apple](https://help.apple.com/applestyleguide/#/apsg1a3a0436), [UK NCSC](https://www.ncsc.gov.uk/blog-post/terminology-its-not-black-and-white)| 
| Blacklist	                            | Blocklist, denylist, unapproved list, reject list, refused, prohibited  |[Bluetooth SIG](https://btprodspecificationrefs.blob.core.windows.net/language-mapping/Appropriate_Language_Mapping_Table.pdf?mkt_tok=eyJpIjoiTWpOa05EUmxaV1EwWkRWbCIsInQiOiJnR1dHMEdpeWNTcXFVdG92NVhGNkVPcEdmWGFlOWppbmMzNVBJdkZiTUxXRjk5bG5cL3RHV3JaSDBXRVhkMThoMEVFRVZ3NXlzOUYyekl4TmJmVlM4bHJZa0x4YmowdFJjOTBMT2d4b2l4eEMxcmIya0FcL1IrSk5KVGw2OVZGV1wvXC8ifQ%3D%3D), [3GPP](https://datatracker.ietf.org/liaison/1716/), [W3C](https://w3c.github.io/manual-of-style/#inclusive), [Avnu](https://avnu.org/wp-content/uploads/2014/05/Avnu_Open-Letter_Inclusive-Terminology-and-Language_July-14-2020_Final.pdf), [Linux](https://git.kernel.org/pub/scm/linux/kernel/git/torvalds/linux.git/commit/?id=49decddd39e5f6132ccd7d9fdc3d7c470b0061bb), [Ansible](https://www.redhat.com/en/blog/making-open-source-more-inclusive-eradicating-problematic-language), [Twitter](https://twitter.com/TwitterEng/status/1278733305190342656), [Chromium](https://chromium.googlesource.com/chromium/src/+/master/styleguide/inclusive_code.md), [Apple](https://help.apple.com/applestyleguide/#/apsg1a3a0436), [UK NCSC](https://www.ncsc.gov.uk/blog-post/terminology-its-not-black-and-white)| 
| Master/slave                          | Leader/follower, primary/replica, primary/secondary, active/standby, main/secondary, leader/follower, orchestrator/worker, initiator/responder, central/peripheral, server/client  |[Bluetooth SIG](https://btprodspecificationrefs.blob.core.windows.net/language-mapping/Appropriate_Language_Mapping_Table.pdf?mkt_tok=eyJpIjoiTWpOa05EUmxaV1EwWkRWbCIsInQiOiJnR1dHMEdpeWNTcXFVdG92NVhGNkVPcEdmWGFlOWppbmMzNVBJdkZiTUxXRjk5bG5cL3RHV3JaSDBXRVhkMThoMEVFRVZ3NXlzOUYyekl4TmJmVlM4bHJZa0x4YmowdFJjOTBMT2d4b2l4eEMxcmIya0FcL1IrSk5KVGw2OVZGV1wvXC8ifQ%3D%3D), [3GPP](https://datatracker.ietf.org/liaison/1716/), [W3C](https://w3c.github.io/manual-of-style/#inclusive), [Avnu](https://avnu.org/wp-content/uploads/2014/05/Avnu_Open-Letter_Inclusive-Terminology-and-Language_July-14-2020_Final.pdf), [Linux](https://git.kernel.org/pub/scm/linux/kernel/git/torvalds/linux.git/commit/?id=49decddd39e5f6132ccd7d9fdc3d7c470b0061bb), [Twitter](https://twitter.com/TwitterEng/status/1278733305190342656), [Apple](https://help.apple.com/applestyleguide/#/apsg72b28652), [Microsoft](https://docs.microsoft.com/en-us/style-guide/bias-free-communication), [Python](https://bugs.python.org/issue34605), [Postgres](https://www.postgresql.org/message-id/flat/E393EC88-377F-4C59-A67A-69F2A38D17C7@yesql.se), [Redis](https://github.com/redis/redis/issues/5335), [Django](https://github.com/django/django/pull/2692), [Drupal](https://www.drupal.org/node/2275877), [CouchDB](https://issues.apache.org/jira/browse/COUCHDB-2248)|
| Master (e.g., branch, key, server)    | Main, parent, server, central                              |[Bluetooth SIG](https://btprodspecificationrefs.blob.core.windows.net/language-mapping/Appropriate_Language_Mapping_Table.pdf?mkt_tok=eyJpIjoiTWpOa05EUmxaV1EwWkRWbCIsInQiOiJnR1dHMEdpeWNTcXFVdG92NVhGNkVPcEdmWGFlOWppbmMzNVBJdkZiTUxXRjk5bG5cL3RHV3JaSDBXRVhkMThoMEVFRVZ3NXlzOUYyekl4TmJmVlM4bHJZa0x4YmowdFJjOTBMT2d4b2l4eEMxcmIya0FcL1IrSk5KVGw2OVZGV1wvXC8ifQ%3D%3D), [W3C](https://w3c.github.io/manual-of-style/#inclusive), [Linux](https://git.kernel.org/pub/scm/linux/kernel/git/torvalds/linux.git/commit/?id=49decddd39e5f6132ccd7d9fdc3d7c470b0061bb), [Ansible](https://www.redhat.com/en/blog/making-open-source-more-inclusive-eradicating-problematic-language), [GitLab](https://gitlab.com/gitlab-org/gitlab/-/issues/221164), [Python](https://bugs.python.org/issue34605), [Redis](https://github.com/redis/redis/issues/5335), [Mozilla](https://bugzilla.mozilla.org/show_bug.cgi?id=1644807)|            
| Grandfathered	                        | Legacy status, historical                                     |[W3C](https://w3c.github.io/manual-of-style/#inclusive), [Twitter](https://twitter.com/TwitterEng/status/1278733305190342656)|
| Gendered terms (e.g., guys)           | People, game clients, players                                 |[Twitter](https://twitter.com/TwitterEng/status/1278733305190342656), [Microsoft](https://docs.microsoft.com/en-us/style-guide/bias-free-communication), [Chromium](https://chromium.googlesource.com/chromium/src/+/master/styleguide/inclusive_code.md)|
| Gendered pronouns (e.g., he/him/his)  | They, them, their, client, player                                 |[W3C](https://w3c.github.io/manual-of-style/#inclusive), [Twitter](https://twitter.com/TwitterEng/status/1278733305190342656), [Google](https://developers.google.com/style/inclusive-documentation), [Microsoft](https://docs.microsoft.com/en-us/style-guide/bias-free-communication), [Chromium](https://chromium.googlesource.com/chromium/src/+/master/styleguide/inclusive_code.md)|
| Man hours	                            | Person hours, engineer hours, staff hours         |[Twitter](https://twitter.com/TwitterEng/status/1278733305190342656), [Google](https://developers.google.com/style/inclusive-documentation)|
| Sanity check                          | Quick check, confidence check, coherence check    |[W3C](https://w3c.github.io/manual-of-style/#inclusive), [Twitter](https://twitter.com/TwitterEng/status/1278733305190342656), [Google](https://developers.google.com/style/inclusive-documentation)|
| Crazy                                 | Unexpected, surprising, puzzling                  |[Google](https://developers.google.com/style/inclusive-documentation)|
| Dummy value                           | Placeholder value, sample value                   |[W3C](https://w3c.github.io/manual-of-style/#inclusive), [Twitter](https://twitter.com/TwitterEng/status/1278733305190342656), [Google](https://developers.google.com/style/inclusive-documentation)|
| Dark Pattern                          | Deception Pattern                                 |[HTC](https://community.humanetech.com/t/5012), [Fediverse](https://mastodon.social/web/statuses/104980550422206716), [Twitter](https://twitter.com/humane_tech_now/status/1312997688770732032)|

## Code of Conduct

### Pledge
In the interest of fostering an open and welcoming environment, we as contributors and maintainers pledge to making participation in our project and our community a harassment-free experience for everyone, regardless of age, body size, disability, ethnicity, gender identity and expression, level of experience, nationality, personal appearance, race, religion, or sexual identity and orientation.

### Standards
Examples of behavior that contributes to creating a positive environment include:

- Using welcoming and inclusive language
- Being respectful of differing viewpoints and experiences
- Gracefully accepting constructive criticism
- Focusing on what is best for the community or project
- Showing empathy towards other community members

Examples of unacceptable behavior by participants include:

- The use of sexualized language or imagery and unwelcome sexual attention or advances
- Trolling, insulting/derogatory comments, and personal or political attacks
- Public or private harassment
- Publishing others' private information, such as a physical or electronic address, without explicit permission
- Other conduct which could reasonably be considered inappropriate in a professional setting

### Responsibilities
Project maintainers are responsible for clarifying the standards of acceptable behavior and are expected to take appropriate and fair corrective action in response to any instances of unacceptable behavior.

Project maintainers have the right and responsibility to remove, edit, or reject comments, commits, code, wiki edits, issues, and other contributions that are not aligned to this Code of Conduct, or to ban temporarily or permanently any contributor for other behaviors that they deem inappropriate, threatening, offensive, or harmful.

### Scope
This Code of Conduct applies both within project spaces and in public spaces when an individual is representing the project or its community. Examples of representing a project or community include using an official project e-mail address, posting via an official social media account, or acting as an appointed representative at an online or offline event. Representation of a project may be further defined and clarified by project maintainers.

### Enforcement
Instances of abusive, harassing, or otherwise unacceptable behavior may be reported by contacting the project team leader [here](https://spirited.io/). All complaints will be reviewed and investigated and will result in a response that is deemed necessary and appropriate to the circumstances. The project team is obligated to maintain confidentiality with regard to the reporter of an incident. Further details of specific enforcement policies may be posted separately.

Project maintainers who do not follow or enforce the Code of Conduct in good faith may face temporary or permanent repercussions as determined by other members of the project's leadership.

### Attribution
This Code of Conduct is adapted from the Contributor Covenant, version 1.4, available at http://contributor-covenant.org/version/1/4.

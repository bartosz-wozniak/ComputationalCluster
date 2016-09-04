# Rules

Subject to change in the future.

## Communication, texts and management

 1. We use English across the whole project, except Slack and documentation that will be handed to teachers,
 2. All communication is done on Slack, in-person meetings or on Skype; Facebook communication is disallowed,
 3. All important decisions (exception: tasks) have to be saved to Trello,
 4. All coding tasks have to have corresponding issues on project's GitLab,
 5. When starting working on a task, you have to assign yourself to the issue,
 6. If issue is already taken, you cannot assign yourself to it without current assignee consent,
 7. When work is finished, you have to open merge request and assign the next person from *Review Rotation* list as a person responsible for code review. This person should then be moved to the end of the list,
 8. After merging a request, write a summary of the task as a comment to the issue (eg. time consumed),
 9. Documentation, tasks, etc. is written in [CommonMark](http://commonmark.org/).

## Code, repository

 1. Disobeying following rules will be punished,
 2. We adhere to `The seven rules of a great git commit message`,
 3. We adapt Gitflow workflow; naming:
    * `master` and `dev`,
    * `release/*`,
    * `feature/*`,
 4. We strongly follow decided coding standards,
 5. `master` may be changed only by the manager,
 6. Changes to `dev` are done using pull requests,
 7. A pull request that will not compile or will not pass unit tests will be rejected,
 8. Each pull request has to be reviewed by at least one person,
 9. Merging pull requests is done by the person reviewing the feature,
 10. We try to make features very small, so that each pull request consists of at most a few commits,
 11. Everyone is encouraged to review others' work without special order.
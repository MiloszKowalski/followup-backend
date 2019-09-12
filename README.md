# FollowUP backend server
A backend server for FollowUP Agency
---

# Architecture

1. Onion architecture, similar to *Passenger* project:
  - **FollowUP.Core** -> Domain models (promotion, direct, comment etc.), Repositiories
  - **FollowUP.Infrastructure** -> Database, EF, Services, Extensions, Repositories, DTO, Commands, Command Handlers, IoC Modules, Exceptions, Settings
  - **FollowUP.API** -> Controllers

2. Framework -> ASP.Net Core 2.2

3. Database -> SQL Server/SQLite [^1]
[^1]: Depends on either we want it to be crossplatform or not

4. Libraries -> Autofac, AutoMapper, Newtonsoft.Json, InstaApiSharp

---
# Data architecture and functionality

### For each:
- User -> UserProfile -> ( Promotions -> stats from repository, flags for on/off; Comments(no need to store, only to cache); Direct; Autoposting; Stats [repository of all stats]; )

1. User Register and Login -> ASP.Net Core Identity
  - JWT Authentication
  - roles: admin, user

2. Managing profiles (data for each profile)
  - Getting available services via Claims on server side

3. Promotions
  - For each profile create additional bot,
  - Get proxy from field,

4. Comments scraping and pagination with proper id
  - Get comments from post OR from user endpoint (to check)
  - Get proper fields (username, email, profile picture, time of comment post)
  - Get comment child comments (recursion?),
  - Put all data into DTO and expose it to controller.

5. Making orders (make it automatic via flags [if isTrialAvailable, add 5 days to services expiration, set isTrialAvailable to false])

6. Adding funds to wallet via PayU/PayPal/Funds transfer -> Stored in database

7. Live admin panel with login (admin role in Identity) # LOW PRIORITY
---
# Tests
###### Low priority, make them when we have time for this
- Unit tests,
- E2E tests.
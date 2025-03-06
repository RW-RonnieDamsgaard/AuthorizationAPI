# AuthorizationAPI

AuthorizationAPI is an ASP.NET Core Web API that provides functionalities for managing articles and comments. The API supports different roles with specific permissions:
- **Editor**: Can edit and delete articles, and edit and delete user comments.
- **Writer**: Can create and edit their own articles.
- **Subscriber**: Can comment on articles.
- **Guest**: Can read articles.

## Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or any other C# IDE

## Getting Started

1. **Clone the repository**:

`git clone https://github.com/RW-RonnieDamsgaard/AuthorizationAPI.git`
`cd AuthorizationAPI\AuthorizationAPI`

2. **Build and run the project**:

`dotnet build`

`dotnet run`


3. **Open Swagger UI**:
    Open your browser and navigate to `http://localhost:<port>/swagger` to access the Swagger UI. The port number will be displayed in the terminal when you run the project.

## API Endpoints

### Authentication

1. **Login**:
    - **Endpoint**: `POST /api/auth/login`
    - **Description**: Authenticates a user and returns a JWT token.
    - **Request Body**:

`
{
    "username": "string",
    "password": "string"
}
`


### Articles

1. **Get Articles**:
    - **Endpoint**: `GET /api/article/articles`
    - **Description**: Retrieves all articles. Accessible to all users.

2. **Create Article** (Writer):
    - **Endpoint**: `POST /api/article/create-article`
    - **Description**: Creates a new article.
    - **Request Body**:

`
{
    "title": "string",
    "content": "string"
}
`

4. **Delete Article** (Editor):
    - **Endpoint**: `DELETE /api/article/delete-article/{id}`
    - **Description**: Deletes an article.

### Comments

1. **Get Comments**:
    - **Endpoint**: `GET /api/article/comments/{articleId}`
    - **Description**: Retrieves comments for a specific article. Accessible to all users.

2. **Comment on Article** (Subscriber):
    - **Endpoint**: `POST /api/article/comment/{articleId}`
    - **Description**: Adds a comment to an article.
    - **Request Body**:

`
{
    "content": "string"
}
`

3. **Delete Comment** (Editor):
    - **Endpoint**: `DELETE /api/article/delete-comment/{id}`
    - **Description**: Deletes a comment.

## Testing with Swagger

1. **users**:
    - Subscriber (username: testuser, password: password)
    - Writer (username: writeruser, password: password)
    - Writer (username: writer2user, password: password)
    - Editor (username: editoruser, password: password)

2. **Login**:
    - Go to the `POST /api/auth/login` endpoint.
    - Enter the username and password for an Editor, Writer, writer2 or Subscriber.
    - Click "Execute" to get the JWT token.
    - Copy the token

3. **Authorize**:
    - Click on the "Authorize" button in the Swagger UI.
    - Enter the JWT token in the format `Bearer <token>`.
    - Click "Authorize" to authenticate.
      
![image](https://github.com/user-attachments/assets/3864f3ef-9741-427a-ac6b-e9585f786358)
![image](https://github.com/user-attachments/assets/c6cc86d7-aa90-400d-990c-566ca901e12e)

4. **Test Endpoints**:
    - **Editor**:
        - Edit an article: `PUT /api/article/edit-article/{id}`
        - Delete an article: `DELETE /api/article/delete-article/{id}`
        - Delete a comment: `DELETE /api/article/delete-comment/{id}`
    - **Writer**:
        - Create an article: `POST /api/article/create-article`
        - Edit an article: `PUT /api/article/edit-article/{id}`
    - **Writer2** after writer have created article:
        - Edit an article thats not your own: `PUT /api/article/edit-article/{id}`
    - **Subscriber**:
        - Comment on an article: `POST /api/article/comment/{articleId}`
    - **Guest**:
        - Read articles: `GET /api/article/articles`
        - Read comments: `GET /api/article/comments/{articleId}`

Remember to test in one session as data is not persistant

    

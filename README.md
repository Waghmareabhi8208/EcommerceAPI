Ecommerce API
A production-ready E-Commerce Backend API built with ASP.NET Core 8, Entity Framework Core, PostgreSQL, Redis, JWT Authentication, and Docker. The application supports user authentication, role-based authorization, product management, cart management, order processing, refresh tokens, pagination, filtering, sorting, and cloud deployment.
Live Demo
Swagger Documentation
https://ecommerceapi-production-9bfd.up.railway.app/swagger
________________________________________
Features
Authentication & Authorization
•	User Registration
•	User Login
•	JWT Access Tokens
•	Refresh Tokens
•	Logout Functionality
•	Current User Endpoint
•	Role-Based Authorization (Admin / Customer)
•	BCrypt Password Hashing
Product Management
•	Create Product (Admin)
•	Update Product (Admin)
•	Delete Product (Admin)
•	Get Product By Id
•	Get All Products
•	Product Image Upload
•	Product Image Delete
Product Search & Filtering
•	Search Products
•	Filter By Price Range
•	Filter By Stock Availability
•	Sorting
•	Pagination
Cart Management
•	Add Product To Cart
•	Update Cart Quantity
•	Remove Product From Cart
•	View User Cart
Order Management
•	Place Order
•	Order Items
•	Shipping Address
•	Order History
Security
•	JWT Authentication
•	Role-Based Authorization
•	Password Hashing
•	Refresh Token Storage In Redis
•	Rate Limiting
Infrastructure
•	PostgreSQL Database
•	Redis Cache
•	Docker Support
•	Railway Deployment
•	Serilog Logging
________________________________________
Tech Stack
Backend
•	ASP.NET Core 8 Web API
•	C#
Database
•	PostgreSQL
•	Entity Framework Core
Authentication
•	JWT Bearer Authentication
•	Refresh Tokens
Caching
•	Redis
•	StackExchange.Redis
Logging
•	Serilog
Deployment
•	Docker
•	Railway
________________________________________
Architecture
User | v ASP.NET Core API | +–> PostgreSQL | - Users | - Products | - Orders | - Cart | +–> Redis - Refresh Tokens
________________________________________
API Endpoints
Authentication
Method	Endpoint	Description
POST	/api/Auth/register	Register User
POST	/api/Auth/login	Login User
POST	/api/Auth/refresh	Refresh Access Token
POST	/api/Auth/logout	Logout User
GET	/api/Auth/me	Current User
Products
Method	Endpoint	Description
GET	/api/Product	Get Products
GET	/api/Product/{id}	Get Product By Id
POST	/api/Product	Add Product (Admin)
PUT	/api/Product/{id}	Update Product (Admin)
DELETE	/api/Product/{id}	Delete Product (Admin)
POST	/api/Product/{id}/upload-image	Upload Product Image (Admin)
DELETE	/api/Product/{id}/image	Delete Product Image (Admin)
________________________________________
Product Query Parameters
Search
GET /api/Product?search=iphone
Price Filter
GET /api/Product?minPrice=100&maxPrice=1000
Stock Filter
GET /api/Product?inStock=true
Sorting
GET /api/Product?sortBy=price
Pagination
GET /api/Product?pageNumber=1&pageSize=10
________________________________________
Database
Main Tables
•	Users
•	Products
•	Carts
•	CartItems
•	Orders
•	OrderItems
•	Addresses
________________________________________
Running Locally
Clone Repository
git clone https://github.com/Waghmareabhi8208/EcommerceAPI.git
cd Ecommerce.API
Restore Packages
dotnet restore
Apply Migrations
Update-Database
Run Application
dotnet run
________________________________________
Environment Variables
PostgreSQL
ConnectionStrings__DefaultConnection=<your-postgresql-connection-string>
JWT
Jwt__Key=<your-jwt-secret-key>
Jwt__Issuer=<your-issuer>
Jwt__Audience=<your-audience>
Redis
REDISHOST=<redis-host>
REDISPORT=<redis-port>
REDISPASSWORD=<redis-password>
Razorpay
Razorpay__Key=<your-razorpay-key>
Razorpay__Secret=<your-razorpay-secret>
________________________________________
Deployment
The application is deployed on Railway using:
•	Docker
•	PostgreSQL Service
•	Redis Service
•	GitHub Continuous Deployment
Every push to the main branch automatically triggers a new deployment.
________________________________________
Future Improvements
•	Product Reviews & Ratings
•	Wishlist
•	Coupons & Discounts
•	Email Notifications
•	Order Tracking
•	Cloud Storage Integration
•	Unit & Integration Tests
________________________________________
Author
Developed as a learning-focused production-grade backend project demonstrating modern ASP.NET Core development, authentication, caching, database management, cloud deployment, and scalable API architecture.

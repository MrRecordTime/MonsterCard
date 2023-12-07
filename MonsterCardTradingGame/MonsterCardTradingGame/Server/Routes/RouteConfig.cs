﻿using System.Net;
using System.Text;
using System.Text.Json;
using MonsterCardTradingGame.BusinessLogic;
using MonsterCardTradingGame.DataBase.Repositories;
using MonsterCardTradingGame.Models.BaseClasses;

namespace MonsterCardTradingGame.Server.Routes
{
    internal class RouteConfig
    {
        private Router _router;
        private GameManager _gameManager;
        public Package _package;

        public RouteConfig(Router router)
        {
            _router = router;
            _gameManager = new GameManager();
            DefineRoutes();
        }

        private void DefineRoutes()
        {
            //Default Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET","/", (requestbody,requestParameter) =>
            {
                return new ResponseGenerator().GenerateResponse();
            });

            //UserDataRoute
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET", "/users", (requestbody, requestParameter) =>
            {
                UserRepository newUserRep = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
                var userData = newUserRep.GetUserData(requestParameter);

                StringBuilder userDataString = new StringBuilder();
                StringBuilder cardDataString = new StringBuilder();
                int cardCount = 1;

                foreach (var kvp in userData)
                {
                    if (kvp.Value is Dictionary<string, object> cardData)
                    {
                        cardDataString.AppendLine($"<b>Card-{cardCount}:</b><br>");
                        foreach (var cardKvp in cardData)
                        {
                            cardDataString.AppendLine($"{cardKvp.Key}: {cardKvp.Value}<br>");
                        }
                        cardCount++;
                    }
                    else
                    {
                        userDataString.AppendLine($"{kvp.Key}: {kvp.Value}<br>");
                        
                    }
                }
                userDataString.AppendLine("<b>CARDS</b><br>");

                string finalOutput = userDataString.ToString() + cardDataString.ToString();

                return $"HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>User Data for {requestParameter}</h1><p>{finalOutput}</p></body></html>";
            });

            // CreateUser Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/users", (requestBody, requestParameter) =>
            {
                try
                {
                    var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

                    if (userData == null || !userData.TryGetValue("Username", out var username) || !userData.TryGetValue("Password", out var password))
                    {
                        return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + JsonSerializer.Serialize(new { Message = "Missing Username or Password" });
                    }

                    UserRepository userRepository = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
                    userRepository.AddUser(username, password);

                    return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + JsonSerializer.Serialize(new { Message = "User created successfully" });
                }
                catch (Exception ex)
                {
                    return "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + JsonSerializer.Serialize(new { Message = $"An error occurred: {ex.Message}" });
                }
            });

            //UserupdatePut Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("PUT", "/users", (requestBody, requestParameter) =>
            {
                try
                {
                    var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

                    if (userData == null || !userData.TryGetValue("Username", out var username) || !userData.TryGetValue("Password", out var password))
                    {
                        return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + JsonSerializer.Serialize(new { Message = "Missing Username or Password" });
                    }

                    UserRepository userRepository = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
                    userRepository.UpdateUser(username,requestParameter, password);

                    return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + JsonSerializer.Serialize(new { Message = "User updated successfully" });
                }
                catch (Exception ex)
                {
                    return "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + JsonSerializer.Serialize(new { Message = $"An error occurred: {ex.Message}" });
                }
            });

            //Login Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/sessions", (requestbody, requestParameter) =>
            {
                return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>" + "Sorry not enough Coins" + "</h1></body></html>";
            });
        }
    }
}
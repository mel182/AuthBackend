﻿namespace WebApplicationAPI.Model
{
    public class ErrorMessage
    {
        public string Message { get; private set; } = "";

        public ErrorMessage(string message)
        {
            this.Message = message;
        }
    }
}

﻿using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace WpfClient;

public partial class MainWindow : Window
{

    HubConnection connection;
    public MainWindow()
    {
        InitializeComponent();

        connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7140/chathub")
            .WithAutomaticReconnect()
            .Build();

        connection.Reconnecting += (sender) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = "Attempting to reconnect...";
                messages.Items.Add(newMessage);
            });

            return Task.CompletedTask;
        };

        connection.Reconnected += (sender) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = "Reconnected to the server";
                messages.Items.Clear();
                messages.Items.Add(newMessage);
            });

            return Task.CompletedTask;
        };

        connection.Closed += (sender) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = "Connection closed";
                messages.Items.Add(newMessage);
                openConnection.IsEnabled = true;
                sendMessage.IsEnabled = false;
            });

            return Task.CompletedTask;
        };
    }

    private async void openConnection_Click(object sender, RoutedEventArgs e)
    {

        // cand clientul a primit un mesaj de tipul ReceiveMessage de la server, il handleuieste
        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = $"{user}: {message}";
                messages.Items.Add(newMessage);
            });
        });

        try
        {
            await connection.StartAsync();
            messages.Items.Add("Connection Started");
            openConnection.IsEnabled = false;
            sendPrivateMessage.IsEnabled = true;
            sendMessage.IsEnabled = true;
        }
        catch(Exception ex)
        {
            messages.Items.Add($"{ex.Message}");
        }
    }

    private async void sendMessage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await connection.InvokeAsync("SendMessage", "WPF Client", messageInput.Text); 
        }
        catch(Exception ex )
        {
            messages.Items.Add($"{ex.Message}");

        }
    }

    private async void sendPrivateMessage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await connection.InvokeAsync("SendMessageToCaller", "WPF Client", messageInput.Text);
        }
        catch (Exception ex)
        {
            messages.Items.Add($"{ex.Message}");

        }
    }
}

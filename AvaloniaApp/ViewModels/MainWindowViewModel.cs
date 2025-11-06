using AvaloniaApp.Models;
using AvaloniaApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Playlist> playlists = new();

        [ObservableProperty]
        private Playlist? selectedPlaylist;

        [ObservableProperty]
        private string playlistName = "";

        [ObservableProperty]
        private string playlistDescription = "";

        [ObservableProperty]
        private string playlistAvatar = "";

        [ObservableProperty]
        private bool isPageLoaded;

        // Этот метод автоматически будет вызван при изменении playlistAvatar
        partial void OnPlaylistAvatarChanged(string value)
        {
            IsPageLoaded = !string.IsNullOrEmpty(value);
        }

        private readonly HttpClient httpClient = new();

        public ObservableCollection<Song> Songs { get; set; } = new()
        {
            /*new Song { SongName = "Thunderstruck", ArtistName = "AC/DC", AlbumName = "The Razors Edge", Duration = "4:52" },
            new Song { SongName = "Nothing Else Matters", ArtistName = "Metallica", AlbumName = "Metallica", Duration = "6:28" },
            new Song { SongName = "Comfortably Numb", ArtistName = "Pink Floyd", AlbumName = "The Wall", Duration = "6:22" },
            new Song { SongName = "Bohemian Rhapsody", ArtistName = "Queen", AlbumName = "A Night at the Opera", Duration = "5:55" },*/
        };

        public MainWindowViewModel()
        {
            Playlists = new ObservableCollection<Playlist>
            {
                new() { Name = "All Hits", Url = "https://music.amazon.com/playlists/B01M11SBC8", Description = "", IsPlaylists = true },
                new() { Name = "Violator", Url = "https://music.amazon.com/albums/B073JC7DCF", Description = "" }
            };

            SelectedPlaylist = Playlists.FirstOrDefault();
            /*if (SelectedPlaylist != null)
                LoadPlaylistAsync(SelectedPlaylist);*/
        }

        partial void OnSelectedPlaylistChanged(Playlist? value)
        {
            if (value != null)
                LoadPlaylistAsync(value);
        }

        private async void LoadPlaylistAsync(Playlist playlist)
        {
            PlaylistName = playlist.Name;
            if (!playlist.IsLoaded)
            {
                PlaylistAvatar = "";
                PlaylistDescription = $"Loading for {playlist.Name}...";
                if (playlist.IsPlaylists)
                {
                    await AmazonPlaylistParser.GetPlaylistAsync(playlist);
                }
                else
                {
                    await AmazonAlbumsParser.GetAlbumsAsync(playlist);
                }
            }

            if (!string.IsNullOrEmpty(playlist.AvatarUrl))
            {
                PlaylistAvatar = playlist.AvatarUrl;
                PlaylistName = playlist.Name;
                PlaylistDescription = playlist.Description;
                Songs.Clear();
                foreach (var song in playlist.Songs)
                    Songs.Add(song);
            }
            else
            {
                PlaylistDescription = "Invalid load";
            }
        }
    }
}

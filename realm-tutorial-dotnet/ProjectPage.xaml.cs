﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using RealmDotnetTutorial.Models;
using Realms;
using Realms.Sync;
using Xamarin.Forms;
using AsyncTask = System.Threading.Tasks.Task;
using User = RealmDotnetTutorial.Models.User;

namespace RealmDotnetTutorial
{
    public partial class ProjectPage : ContentPage
    {
        private User user;
        private Realm userRealm;
        private ObservableCollection<Project> _projects = new ObservableCollection<Project>();

        public ObservableCollection<Project> MyProjects
        {
            get
            {
                return _projects;
            }
        }

        public ProjectPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            //WaitingLayout.IsVisible = true;
            if (App.RealmApp.CurrentUser == null)
            {
                // No user? Go back to the LoginPage
                await Navigation.PopAsync();
            }
            else
            {
                await LoadProjects();
            }
            base.OnAppearing();
        }

        private async AsyncTask LoadProjects()
        {
            try
            {
                //var syncConfig = new SyncConfiguration($"user={ App.RealmApp.CurrentUser.Id }", App.RealmApp.CurrentUser);
                App.gsky_pk = $"user={ App.gsky_RealmUser.Id }";
                var syncConfig = new SyncConfiguration(App.gsky_pk, App.gsky_RealmUser);
                userRealm = await Realm.GetInstanceAsync(syncConfig);
                user = userRealm.Find<User>(App.gsky_RealmUser.Id);
                if (user != null)
                {
                    SetUpProjectList();
                } else
                {
                    MyProjects.Clear();
                    listProjects.ItemsSource = MyProjects;
                    MyProjects.Add(new Project("Why is user null?"));
                    MyProjects.Add(new Project("User pk is " + App.gsky_pk));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error Loading Projects", ex.Message, "OK");
            }
        }

        private async void MemberPage_OperationCompeleted(object sender, EventArgs e)
        {
            (sender as AddMemberPage).OperationCompeleted -= MemberPage_OperationCompeleted;
            await LoadProjects();
        }

        private void SetUpProjectList()
        {
            MyProjects.Clear();
            listProjects.ItemsSource = MyProjects;
            foreach (Project p in user.MemberOf)
            {
                MyProjects.Add(p);
            }
            if (MyProjects.Count <= 0)
            {
                MyProjects.Add(new Project("No projects found!"));
            }

//; WaitingLayout.IsVisible = false;
        }

        void TextCell_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TaskPage());
        }

        async void Add_User_Button_Clicked(object sender, EventArgs e)
        {
            var memberPage = new AddMemberPage();
            memberPage.OperationCompeleted += MemberPage_OperationCompeleted;
            await Navigation.PushAsync(memberPage);
        }

        async void Logout_Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (App.RealmApp.CurrentUser != null)
                {
                    await App.RealmApp.CurrentUser.LogOutAsync();
                    var loginPage = new LoginPage();
                    await Navigation.PushAsync(loginPage);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Logout Failed");
            }
        }
    }
}
<Window x:Class="NeuUITest.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="clr-namespace:NeuUITest.ViewModels"
        xmlns:controls="clr-namespace:NeuUITest.J.TestSummary"
        Title="Unit Test Runner" Height="700" Width="1000">

    <!-- Binding file -->
    <Window.DataContext>
        <vm:MainMainWindow />
    </Window.DataContext>

    <Grid Margin="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>  <!-- Menu -->
            <RowDefinition Height="Auto"/>  <!-- Toolbar -->
            <RowDefinition Height="2*"/>    <!-- Console Output -->
            <RowDefinition Height="2*"/>    <!-- Summary Results -->
            <RowDefinition Height="Auto"/>  <!-- Test Summary Control -->
        </Grid.RowDefinitions>

        <!-- MENU -->
        <Menu Grid.Row="0">
            <MenuItem Header="_Datei">
                <MenuItem Header="Neuer Test" Click="MenuItem_NeuerTest"/>
                <MenuItem Header="Laden" Click="MenuItem_Laden"/>
                <MenuItem Header="Speichern" Click="MenuItem_Speichern"/>
                <Separator />
                <MenuItem Header="Beenden" Click="MenuItem_Beenden"/>
            </MenuItem>
            <MenuItem Header="_Projekt">
                <MenuItem Header="Option 1" Click="MenuItem_ProjektOption1"/>
            </MenuItem>
            <MenuItem Header="_Auswertung"/>
            <MenuItem Header="_Warten"/>
            <MenuItem Header="_Server"/>
        </Menu>

        <!-- TOOLBAR -->
        <StackPanel Orientation="Horizontal" Grid.Row="1" Margin="0,0,10,0">
            <Button Content="Browse DLL" Command="{Binding BrowseDLLViewModel.BrowseDLLCommand}" />
            <TextBox Text="{Binding BrowseDLLViewModel.DLLPath}" Width="600" Margin="10,0" IsReadOnly="True"/>
            <Button Content="Run Tests" Width="100" Command="{Binding RunTestsViewModel.RunTestsCommand}" Margin="10,0,0,0"/>
            <Button Content="About" Width="100" Command="{Binding AboutViewModel.ShowAboutCommand}" Margin="10,0,0,0"/>
        </StackPanel>

        <!-- TERMINAL-STYLE RAW OUTPUT -->
        <ListBox Grid.Row="2"
                 ItemsSource="{Binding ConsoleOutput}"
                 FontFamily="Consolas"
                 FontSize="13"
                 Background="Black"
                 Foreground="LightGray"
                 BorderBrush="Gray"
                 BorderThickness="1" />

        <!-- COLORED SUMMARY OUTPUT -->
        <ListBox Grid.Row="3"
                 ItemsSource="{Binding RunTestsViewModel.SummaryResults}"
                 FontFamily="Consolas"
                 FontSize="13"
                 Background="White"
                 Foreground="Black"
                 BorderBrush="Gray"
                 BorderThickness="1">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding Message}"
                               Foreground="{Binding StatusBrush}" />
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>

        <!-- SUMMARY CONTROL -->
        <controls:TestSummaryControl Grid.Row="4"
                                     Margin="0,10,0,0"
                                     DataContext="{Binding TestSummaryViewModel}" />
    </Grid>
</Window>
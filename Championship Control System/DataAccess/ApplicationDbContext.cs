using Championship_Control_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Championship_Control_System.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Championship> Championships { get; set; }
        public virtual DbSet<Coach> Coaches { get; set; }
        public virtual DbSet<Match> Matches { get; set; }
        public virtual DbSet<MatchEvent> MatchEvents { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Stadium> Stadia { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<TeamStanding> TeamStandings { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<ApplicationUserOTP> ApplicationUserOTP { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ضروري جداً لتشغيل جداول Identity
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Championship>(entity =>
            {
                entity.HasKey(e => e.ChampionshipId).HasName("PK__Champion__0947429778D8F492");
                entity.ToTable("Championship");
                entity.Property(e => e.ChampionshipId).HasColumnName("ChampionshipID");
                entity.Property(e => e.ChampionshipName).HasMaxLength(100);
                entity.Property(e => e.Country).HasMaxLength(50);
                entity.Property(e => e.Logo).HasMaxLength(255);
                entity.Property(e => e.Season).HasMaxLength(20);
            });

            modelBuilder.Entity<Coach>(entity =>
            {
                entity.HasKey(e => e.CoachId).HasName("PK__Coach__F411D9A124C3AD99");
                entity.ToTable("Coach");
                entity.Property(e => e.CoachId).HasColumnName("CoachID");
                entity.Property(e => e.Img).HasMaxLength(255);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.HasOne(d => d.Team).WithOne(p => p.Coach)
                    .HasForeignKey<Team>(t => t.CoachId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(e => e.MatchId).HasName("PK__Match__4218C837F1D022D7");
                entity.ToTable("Match");
                entity.Property(e => e.MatchId).HasColumnName("MatchID");
                entity.Property(e => e.AwayGoals).HasDefaultValue(0);
                entity.Property(e => e.AwayTeamId).HasColumnName("AwayTeamID");
                entity.Property(e => e.ChampionshipId).HasColumnName("ChampionshipID");
                entity.Property(e => e.HomeGoals).HasDefaultValue(0);
                entity.Property(e => e.HomeTeamId).HasColumnName("HomeTeamID");
                entity.Property(e => e.MatchDate).HasColumnType("datetime");
                entity.Property(e => e.StadiumId).HasColumnName("StadiumID");
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.TicketPrice)
                   .HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.AwayTeam).WithMany(p => p.MatchAwayTeams)
                    .HasForeignKey(d => d.AwayTeamId)
                    .HasConstraintName("FK_Match_AwayTeam");

                entity.HasOne(d => d.Championship).WithMany(p => p.Matches)
                    .HasForeignKey(d => d.ChampionshipId)
                    .HasConstraintName("FK_Match_Championship");

                entity.HasOne(d => d.HomeTeam).WithMany(p => p.MatchHomeTeams)
                    .HasForeignKey(d => d.HomeTeamId)
                    .HasConstraintName("FK_Match_HomeTeam");

                entity.HasOne(d => d.Stadium).WithMany(p => p.Matches)
                    .HasForeignKey(d => d.StadiumId)
                    .HasConstraintName("FK_Match_Stadium");
            });

            modelBuilder.Entity<MatchEvent>(entity =>
            {
                entity.HasKey(e => e.EventId).HasName("PK__MatchEve__7944C87044D9C5EC");
                entity.ToTable("MatchEvent");
                entity.Property(e => e.EventId).HasColumnName("EventID");
                entity.Property(e => e.EventType).HasMaxLength(50);
                entity.Property(e => e.MatchId).HasColumnName("MatchID");

                entity.HasOne(d => d.Match).WithMany(p => p.MatchEvents)
                    .HasForeignKey(d => d.MatchId)
                    .HasConstraintName("FK_Event_Match");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.PlayerId).HasName("PK__Player__4A4E74A806D6C4C2");
                entity.ToTable("Player");
                entity.Property(e => e.PlayerId).HasColumnName("PlayerID");
                entity.Property(e => e.Fname).HasMaxLength(50).HasColumnName("FName");
                entity.Property(e => e.Img).HasMaxLength(255);
                entity.Property(e => e.Lname).HasMaxLength(50).HasColumnName("LName");
                entity.Property(e => e.Nationality).HasMaxLength(50);
                entity.Property(e => e.Position).HasMaxLength(30);
                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.HasOne(d => d.Team).WithMany(p => p.Players)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK_Player_Team");
            });

            modelBuilder.Entity<Stadium>(entity =>
            {
                entity.HasKey(e => e.StadiumId).HasName("PK__Stadium__ED83303868D5F43E");
                entity.ToTable("Stadium");
                entity.Property(e => e.StadiumId).HasColumnName("StadiumID");
                entity.Property(e => e.City).HasMaxLength(50);
                entity.Property(e => e.StadiumName).HasMaxLength(100);
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.TeamId).HasName("PK__Team__123AE7B9560400F4");
                entity.ToTable("Team");
                entity.Property(e => e.TeamId).HasColumnName("TeamID");
                entity.Property(e => e.Country).HasMaxLength(50);
                entity.Property(e => e.Logo).HasMaxLength(255);
                entity.Property(e => e.StadiumId).HasColumnName("StadiumID");
                entity.Property(e => e.TeamName).HasMaxLength(100);

                entity.HasOne(d => d.Stadium).WithOne(p => p.Team)
                    .HasForeignKey<Team>(t => t.StadiumId);

                entity.HasMany(d => d.Championships).WithMany(p => p.Teams)
                    .UsingEntity<Dictionary<string, object>>(
                        "TeamChampionship",
                        r => r.HasOne<Championship>().WithMany().HasForeignKey("ChampionshipId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_TC_Championship"),
                        l => l.HasOne<Team>().WithMany().HasForeignKey("TeamId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_TC_Team"),
                        j =>
                        {
                            j.HasKey("TeamId", "ChampionshipId").HasName("PK__Team_Cha__72AE9390DE28B07D");
                            j.ToTable("Team_Championship");
                        });
            });

            modelBuilder.Entity<TeamStanding>(entity =>
            {
                entity.HasKey(e => e.StandingId).HasName("PK__TeamStan__FC2758E130E70C30");
                entity.ToTable("TeamStanding");
                entity.Property(e => e.StandingId).HasColumnName("StandingID");
                entity.Property(e => e.ChampionshipId).HasColumnName("ChampionshipID");
                entity.Property(e => e.GoalDifference).HasDefaultValue(0);
                entity.Property(e => e.Lost).HasDefaultValue(0);
                entity.Property(e => e.Played).HasDefaultValue(0);
                entity.Property(e => e.TeamId).HasColumnName("TeamID");
                entity.Property(e => e.Won).HasDefaultValue(0);

                entity.HasOne(d => d.Championship).WithMany(p => p.TeamStandings)
                    .HasForeignKey(d => d.ChampionshipId)
                    .HasConstraintName("FK_Standing_Championship");

                entity.HasOne(d => d.Team).WithMany(p => p.TeamStandings)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK_Standing_Team");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.TicketId).HasName("PK__Ticket__712CC62700A93A21");
                entity.ToTable("Ticket");

                entity.Property(e => e.TicketId).HasColumnName("TicketID");
                entity.Property(e => e.BookingDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Match).WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.MatchId)
                    .HasConstraintName("FK_Ticket_Match");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Ticket_User");

                entity.Property(e => e.TicketPrice)
                   .HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("CartItem");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.MatchId).HasColumnName("MatchID");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Match)
                    .WithMany()
                    .HasForeignKey(d => d.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                // One row per match per user (instead of duplicate rows)
                entity.HasIndex(e => new { e.UserId, e.MatchId }).IsUnique();
            });



            modelBuilder.Entity<ApplicationUser>()
                .HasMany(d => d.Events).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserSeeEvent",
                    r => r.HasOne<MatchEvent>().WithMany().HasForeignKey("EventId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_See_Event"),
                    l => l.HasOne<ApplicationUser>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_See_User"),
                    j =>
                    {
                        j.HasKey("UserId", "EventId").HasName("PK__User_See__001C802B7224D085");
                        j.ToTable("User_See_Event");
                        j.IndexerProperty<string>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("EventId").HasColumnName("EventID");
                    });
        }
    }
}
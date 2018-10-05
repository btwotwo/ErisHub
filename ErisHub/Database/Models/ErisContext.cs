using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ErisHub.Database.Models
{
    public partial class ErisContext : DbContext
    {
        public ErisContext()
        {
        }

        public ErisContext(DbContextOptions<ErisContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ban> Bans { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<PollOption> PollOptions { get; set; }
        public virtual DbSet<Poll> Polls { get; set; }
        public virtual DbSet<PollTextReply> PollTextReplies { get; set; }
        public virtual DbSet<PollVote> PollVotes { get; set; }
        public virtual DbSet<Population> Populations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ban>(entity =>
            {
                entity.ToTable("bans");

                entity.HasIndex(e => e.BannedById)
                    .HasName("index_bans_on_banned_by_id");

                entity.HasIndex(e => e.TargetId)
                    .HasName("index_bans_on_target_id");

                entity.HasIndex(e => e.UnbannedById)
                    .HasName("index_bans_on_unbanned_by_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BannedById)
                    .HasColumnName("banned_by_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Cid)
                    .HasColumnName("cid")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Duration)
                    .HasColumnName("duration")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ExpirationTime)
                    .HasColumnName("expiration_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .HasColumnName("ip")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Job)
                    .HasColumnName("job")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasColumnName("reason")
                    .HasColumnType("text");

                entity.Property(e => e.Server)
                    .IsRequired()
                    .HasColumnName("server")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.TargetId)
                    .HasColumnName("target_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Time)
                    .HasColumnName("time")
                    .HasColumnType("datetime");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Unbanned)
                    .HasColumnName("unbanned")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.UnbannedById)
                    .HasColumnName("unbanned_by_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UnbannedTime)
                    .HasColumnName("unbanned_time")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.BannedBy)
                    .WithMany(p => p.BansBannedBy)
                    .HasForeignKey(d => d.BannedById)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_rails_20d480679b");

                entity.HasOne(d => d.Target)
                    .WithMany(p => p.BansTarget)
                    .HasForeignKey(d => d.TargetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_rails_62ac37e1e1");

                entity.HasOne(d => d.UnbannedBy)
                    .WithMany(p => p.BansUnbannedBy)
                    .HasForeignKey(d => d.UnbannedById)
                    .HasConstraintName("fk_rails_a305c9e562");
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("books");

                entity.HasIndex(e => e.AuthorId)
                    .HasName("index_books_on_author_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Author)
                    .HasColumnName("author")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.AuthorId)
                    .HasColumnName("author_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Category)
                    .HasColumnName("category")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Content)
                    .HasColumnName("content")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.AuthorNavigation)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("fk_rails_53d51ce16a");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("players");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ByondVersion)
                    .IsRequired()
                    .HasColumnName("byond_version")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Cid)
                    .IsRequired()
                    .HasColumnName("cid")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Ckey)
                    .IsRequired()
                    .HasColumnName("ckey")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.FirstSeen)
                    .HasColumnName("first_seen")
                    .HasColumnType("datetime");

                entity.Property(e => e.Flags)
                    .HasColumnName("flags")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("ip")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.LastSeen)
                    .HasColumnName("last_seen")
                    .HasColumnType("datetime");

                entity.Property(e => e.Rank)
                    .IsRequired()
                    .HasColumnName("rank")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("'player'");

                entity.Property(e => e.Registered)
                    .HasColumnName("registered")
                    .HasColumnType("date");
            });

            modelBuilder.Entity<PollOption>(entity =>
            {
                entity.ToTable("poll_options");

                entity.HasIndex(e => e.PollId)
                    .HasName("index_poll_options_on_poll_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MaxValue)
                    .HasColumnName("max_value")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MinValue)
                    .HasColumnName("min_value")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PollId)
                    .HasColumnName("poll_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnName("text")
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Poll)
                    .WithMany(p => p.PollOptions)
                    .HasForeignKey(d => d.PollId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_rails_aa85becb42");
            });

            modelBuilder.Entity<Poll>(entity =>
            {
                entity.ToTable("polls");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.End)
                    .HasColumnName("end")
                    .HasColumnType("datetime");

                entity.Property(e => e.Question)
                    .IsRequired()
                    .HasColumnName("question")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Start)
                    .HasColumnName("start")
                    .HasColumnType("datetime");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasColumnType("varchar(16)");
            });

            modelBuilder.Entity<PollTextReply>(entity =>
            {
                entity.ToTable("poll_text_replies");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("index_poll_text_replies_on_player_id");

                entity.HasIndex(e => e.PollId)
                    .HasName("index_poll_text_replies_on_poll_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("player_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PollId)
                    .HasColumnName("poll_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasColumnType("text");

                entity.Property(e => e.Time)
                    .HasColumnName("time")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PollTextReplies)
                    .HasForeignKey(d => d.PlayerId)
                    .HasConstraintName("fk_rails_ffc8df499f");

                entity.HasOne(d => d.Poll)
                    .WithMany(p => p.PollTextReplies)
                    .HasForeignKey(d => d.PollId)
                    .HasConstraintName("fk_rails_0833f4df0b");
            });

            modelBuilder.Entity<PollVote>(entity =>
            {
                entity.ToTable("poll_votes");

                entity.HasIndex(e => e.OptionId)
                    .HasName("index_poll_votes_on_option_id");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("index_poll_votes_on_player_id");

                entity.HasIndex(e => e.PollId)
                    .HasName("index_poll_votes_on_poll_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.OptionId)
                    .HasColumnName("option_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("player_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PollId)
                    .HasColumnName("poll_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Time)
                    .HasColumnName("time")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Option)
                    .WithMany(p => p.PollVotes)
                    .HasForeignKey(d => d.OptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_rails_826ebfbbb3");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PollVotes)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_rails_a3e5a3aede");

                entity.HasOne(d => d.Poll)
                    .WithMany(p => p.PollVotes)
                    .HasForeignKey(d => d.PollId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_rails_a6e6974b7e");
            });

            modelBuilder.Entity<Population>(entity =>
            {
                entity.ToTable("populations");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AdminCount)
                    .HasColumnName("admin_count")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PlayerCount)
                    .HasColumnName("player_count")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Server)
                    .IsRequired()
                    .HasColumnName("server")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Time)
                    .HasColumnName("time")
                    .HasColumnType("datetime");
            });
        }
    }
}

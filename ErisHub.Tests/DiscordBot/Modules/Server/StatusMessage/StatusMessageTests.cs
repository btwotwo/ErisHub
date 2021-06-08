using Xunit;
using ErisHub.DiscordBot.Modules.Server;
using System;
using System.Collections.Generic;
using System.Text;
using ErisHub.Tests.Helpers;
using Discord;
using Moq;
using System.Threading.Tasks;
using FluentAssertions;

namespace ErisHub.DiscordBot.Modules.Server.Tests.DiscordBot
{
    public class StatusMessageTests: TestBase
    {
        StatusMessage CreateMessage()
        {
            return new StatusMessage(Consts.ChannelId1, M<IDiscordClient>().Object);
        }

        [Fact]
        public async Task TryCreateFromId_ChannelIdInvalid_Throws()
        {
            M<IDiscordClient>().Setup(x => x.GetChannelAsync(Consts.ChannelId1, CacheMode.AllowDownload, null)).ReturnsAsync(M<IAudioChannel>().Object);

            var msg = CreateMessage();
            Func<Task<ulong>> task = () => msg.TryCreateFromIdAsync(Consts.ChannelId1);

            await task.Should().ThrowAsync<InvalidChannelStatusException>();
        }

        [Theory]
        [InlineData(Consts.InvalidMessageId)]
        [InlineData(Consts.MessageId1)]
        public async Task TryCreateFromId_ChannelValid_MessageIdInvalid_SendsAndReturnsMessageId(ulong messageId)
        {
            SetupValidMocks();

            M<IMessageChannel>().Setup(x => x.GetMessageAsync(Consts.ChannelId1, CacheMode.AllowDownload, null)).ReturnsAsync(M<ISystemMessage>().Object);

            M<IMessageChannel>().Setup(x => x.SendMessageAsync(It.IsAny<string>(), false, null, null, null, null)).ReturnsAsync(M<IUserMessage>().Object);


            M<IUserMessage>().SetupGet(x => x.Id).Returns(Consts.MessageId1);

            var msg = CreateMessage();

            var result = await msg.TryCreateFromIdAsync(messageId);

            result.Should().Be(Consts.MessageId1);
            M<IMessageChannel>().Verify(x => x.SendMessageAsync("Updating...", false, null, null, null, null), Times.Once);
        }


        [Fact]
        public async Task TryCreateFromId_ChannelValid_MessageIdValid_DoesNotSendNewMessage()
        {
            SetupValidMocks();

            M<IMessageChannel>().Setup(x => x.GetMessageAsync(Consts.MessageId1, CacheMode.AllowDownload, null)).ReturnsAsync(M<IUserMessage>().Object);
            M<IUserMessage>().SetupGet(x => x.Id).Returns(Consts.MessageId1);

            var msg = CreateMessage();
            var result = await msg.TryCreateFromIdAsync(Consts.MessageId1);

            result.Should().Be(Consts.MessageId1);

            M<IMessageChannel>().Verify(x => x.SendMessageAsync(It.IsAny<string>(), false, null, null, null, null), Times.Never);
        }

        private void SetupValidMocks()
        {
            M<IDiscordClient>().Setup(x => x.GetChannelAsync(Consts.ChannelId1, CacheMode.AllowDownload, null))
                .ReturnsAsync(M<IMessageChannel>().Object);
        }
    }
}
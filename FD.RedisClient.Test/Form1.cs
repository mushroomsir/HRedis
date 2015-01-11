using System;
using System.Windows.Forms;

namespace FD.RedisClient.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RedisBaseClient redis = new RedisBaseClient();
            var result = redis.SendCommand(RedisCommand.INFO);
            richTextBox1.Text = result.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RedisBaseClient redis = new RedisBaseClient();
            var result = redis.SendCommand(RedisCommand.SET, "msg", "testvalue");
            richTextBox1.Text = result.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RedisBaseClient redis = new RedisBaseClient();
            var result = redis.SendCommand(RedisCommand.GET, "msg");
            richTextBox1.Text = result.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RedisBaseClient redis = new RedisBaseClient();
            richTextBox1.Text = redis.SetByPipeline("button4_Click", "test", 1000);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            RedisBaseClient redis = new RedisBaseClient();
            richTextBox1.Text = redis.Set("button5_Click", "test");
        }

    }
}

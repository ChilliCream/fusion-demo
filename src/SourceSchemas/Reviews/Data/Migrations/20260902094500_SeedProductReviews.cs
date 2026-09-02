using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Reviews.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 4, "Margaret Hamilton" },
                    { 5, "Dennis Ritchie" },
                    { 6, "Barbara Liskov" },
                    { 7, "Linus Torvalds" },
                    { 8, "Donald Knuth" },
                    { 9, "Edsger Dijkstra" },
                    { 10, "Katherine Johnson" },
                    { 11, "Tim Berners-Lee" },
                    { 12, "Frances Allen" },
                    { 13, "Ken Thompson" }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "AuthorId", "Body", "CreateAt", "ProductId", "Stars" },
                values: new object[,]
                {
                    { 5, 4, "After two dinners on it, I am pleased with the Table. Nothing shifts when someone leans on an edge. Even the matte top ignores water rings.", ReviewDate(0, 9, 17), 1, 4 },
                    { 6, 5, "We squeezed a Table into the breakfast nook. Its width left just enough room to pull the chairs back, and the hardware arrived sorted in separate bags.", ReviewDate(6, 18, 42), 1, 4 },
                    { 7, 6, "Heavy legs, no wobble: this Table handles workdays and dinners beautifully.", ReviewDate(12, 11, 8), 1, 5 },
                    { 8, 7, "Seven days in, the initially firm cushions on our new Couch finally relaxed. Delivery staff carried it upstairs carefully. The seat depth is exactly right for stretching out.", ReviewDate(18, 14, 31), 2, 5 },
                    { 9, 8, "Two cats have tested the fabric harder than we have. The Couch has not snagged, the low back is comfortable, and it feels worth the higher price.", ReviewDate(24, 20, 5), 2, 5 },
                    { 10, 9, "For quick dinners this Chair is fine. The compact shape saves room, but the firm seat sends me looking for a cushion after an hour.", ReviewDate(30, 8, 54), 3, 3 },
                    { 11, 10, "Under my desk, the Chair disappears neatly. Its finish matched the photograph better than expected, and there has been no creak from the frame.", ReviewDate(36, 16, 19), 3, 4 },
                    { 12, 11, "A comfortable Chair with good lumbar support, tidy screws, and real value.", ReviewDate(42, 12, 47), 3, 5 },
                    { 13, 12, "Uneven floors usually make shelving a headache. This Bookshelf leveled easily. Its dark finish gives the room a calmer backdrop for books.", ReviewDate(48, 19, 26), 4, 5 },
                    { 14, 13, "Paperbacks fill the Bookshelf without any visible sagging. It is slimmer than my previous unit, while the back panel feels a little too light.", ReviewDate(54, 10, 13), 4, 4 },
                    { 15, 1, "One drawing in the instructions sent me backward, so assembly of the Bookshelf took longer than planned. It stands steady now, although the manual needs work.", ReviewDate(60, 15, 38), 4, 3 },
                    { 16, 2, "Taller art books finally have a home. We anchored the Bookshelf, moved the adjustable shelves twice, and ended up with a very secure unit.", ReviewDate(66, 21, 7), 4, 5 },
                    { 17, 4, "No dents, no chemical smell, no surprise. The Bookshelf arrived in a tall box and the finish is pleasantly smooth rather than plastic-looking.", ReviewDate(72, 9, 45), 4, 4 },
                    { 18, 5, "This Bookshelf stays square under cookbooks and costs less than comparable pieces.", ReviewDate(78, 17, 22), 4, 4 },
                    { 19, 6, "My home office needed room for two monitors, not another giant surface. The Desk strikes that balance. Threading cables underneath made the setup look finished.", ReviewDate(84, 13, 56), 5, 5 },
                    { 20, 7, "Drawer slides are quiet and the Desk feels solid. In a small office, the stated depth leaves a workable path behind my chair.", ReviewDate(90, 20, 11), 5, 4 },
                    { 21, 8, "Better finish than expected; one tight Desk hole was easy to fix.", ReviewDate(96, 8, 33), 5, 4 },
                    { 22, 9, "The drawer diagram is the weak point here. Once I got past that, the Desk worked well enough, though its thin back panel feels cheaper than the rest.", ReviewDate(102, 16, 58), 5, 3 },
                    { 23, 10, "It reached us ahead of schedule with every corner protected. This Desk is a practical, scratch-free workspace at a price I can live with.", ReviewDate(108, 11, 24), 5, 5 },
                    { 24, 11, "Our heavy mattress has not produced a single squeak from the Bed Frame. The slats clicked into place. Getting out of bed no longer wakes the other person.", ReviewDate(114, 18, 49), 6, 5 },
                    { 25, 12, "Getting the Bed Frame upstairs took two people. Assembly afterward was easy, and the upholstered edge is nicer on bare shins than I expected.", ReviewDate(120, 9, 6), 6, 4 },
                    { 26, 13, "The Bed Frame feels properly braced without a box spring, despite unhelpful rail labels.", ReviewDate(126, 14, 42), 6, 4 },
                    { 27, 1, "A scuffed corner arrived on our Bed Frame, but it faces the wall. More importantly, it has stayed quiet and sturdy through several restless weeks.", ReviewDate(132, 21, 15), 6, 3 },
                    { 28, 2, "Clean lines and useful clearance underneath made this Bed Frame work in a tight room. The center support feels reassuring with a larger mattress.", ReviewDate(138, 10, 27), 6, 4 },
                    { 29, 4, "I built the Bed Frame solo before lunch. The even finish and lack of movement when I sit down make it one of the better flat-pack purchases here.", ReviewDate(144, 17, 53), 6, 5 },
                    { 30, 5, "Chargers, a paperback, and reading glasses all fit in the Nightstand drawer. Its height lines up neatly with our mattress. That matters more than I expected.", ReviewDate(150, 8, 16), 7, 4 },
                    { 31, 6, "Nice grain and no sharp corners on the Nightstand. The drawer catches in the final inch, which is irritating but not return-worthy.", ReviewDate(156, 15, 41), 7, 3 },
                    { 32, 7, "Mostly assembled on arrival, our Nightstand felt heavier than expected. Beside the bed, it looks far less like flat-pack furniture than it is.", ReviewDate(162, 19, 9), 7, 5 },
                    { 33, 8, "That awkward sliver beside the bed finally has a purpose. I measured first; the Nightstand fits exactly, and the open shelf catches my glasses before they vanish.", ReviewDate(168, 11, 34), 7, 4 },
                    { 34, 9, "The Nightstand arrived intact, stable, and reasonably priced.", ReviewDate(174, 20, 2), 7, 4 },
                    { 35, 10, "Board-game night is less chaotic now. Snacks wipe straight off the Coffee Table. Controllers and coasters disappear onto the lower shelf.", ReviewDate(180, 9, 28), 8, 5 },
                    { 36, 11, "Aligning one shelf took a second attempt, but the Coffee Table became sturdy afterward. Its proportions suit the sofa instead of crowding the rug.", ReviewDate(186, 16, 52), 8, 4 },
                    { 37, 12, "Chipped Coffee Table corner and a poor finish for the price.", ReviewDate(192, 12, 14), 8, 2 },
                    { 38, 13, "Rounded corners mattered most with a toddler learning to walk. The Coffee Table is heavy enough to stay put when little hands use it for balance.", ReviewDate(198, 18, 37), 8, 5 },
                    { 39, 1, "Check the height before ordering: our Coffee Table sits lower than I pictured. The wood tone is beautiful, so we kept it.", ReviewDate(204, 10, 59), 8, 4 },
                    { 40, 2, "An easy evening build and a month of daily use later, the Coffee Table still feels solid. Sensible value without trying to look fancy.", ReviewDate(210, 21, 21), 8, 4 },
                    { 41, 4, "Long meals are easier in this Dining Chair than in our old set. The curved back supports without pressing into my shoulders. Every leg sits flat on tile.", ReviewDate(216, 8, 43), 9, 4 },
                    { 42, 5, "Nice Dining Chair, but the thin padding limits dinner to an hour.", ReviewDate(222, 15, 5), 9, 3 },
                    { 43, 6, "Spilled sauce wiped off the Dining Chair fabric without leaving a mark. We ordered several, and the stitching and color are remarkably consistent across all of them.", ReviewDate(228, 19, 32), 9, 5 },
                    { 44, 7, "All hardware was sorted before our Dining Chair shipment arrived. Seat height lines up with the table, and the finished chair feels strong.", ReviewDate(234, 11, 57), 9, 4 },
                    { 45, 8, "For a small dining room, the Dining Chair has the right footprint. Straightforward setup and a price low enough that I would order another.", ReviewDate(240, 20, 18), 9, 4 },
                    { 46, 9, "I got useful hanging room from the Wardrobe, but its shelves are shallower than I expected. Budget an afternoon for assembly and do not try it alone.", ReviewDate(246, 9, 39), 10, 3 },
                    { 47, 10, "An open rack made our bedroom look unfinished. This Wardrobe closes evenly, fits below the ceiling by a sensible margin, and tidies the room immediately.", ReviewDate(252, 16, 4), 10, 4 },
                    { 48, 11, "The finished Wardrobe does not sway, which matters because the panels are hefty. Attaching the doors really needs a second person.", ReviewDate(258, 12, 29), 10, 3 },
                    { 49, 12, "Quiet hinges won me over. With the rail adjusted, the Wardrobe holds dresses and shirts neatly, and its white finish wipes clean after dusty weeks.", ReviewDate(264, 18, 51), 10, 4 },
                    { 50, 13, "Every Wardrobe part was labeled and there were no bruised corners. It is plain-looking, admittedly, but the room is much calmer with things behind doors.", ReviewDate(270, 10, 22), 10, 3 },
                    { 51, 1, "Need closed storage quickly? This Wardrobe does the job. The drawers are smaller than anticipated, yet the cabinet itself feels stable and fairly priced.", ReviewDate(276, 21, 8), 10, 4 },
                    { 52, 2, "Cables no longer spill out behind the television. The TV Stand has room for our soundbar and console, with useful openings exactly where they are needed.", ReviewDate(282, 8, 35), 11, 5 },
                    { 53, 4, "Leveling took minutes, then the TV Stand doors lined up cleanly. It sits lower than our old console, just as the listed height warned.", ReviewDate(288, 15, 54), 11, 4 },
                    { 54, 5, "Game console stays cool. Shelves feel firm. This TV Stand was painless to assemble.", ReviewDate(294, 19, 17), 11, 4 },
                    { 55, 6, "We like the finish on the TV Stand, but the supplied wall straps were too short for our setup. The cabinet stands solidly without them.", ReviewDate(300, 11, 46), 11, 3 },
                    { 56, 7, "Well packed and better-looking than its price suggested, the TV Stand hides remotes and cables beautifully. Guests assume it cost more.", ReviewDate(306, 20, 9), 11, 5 },
                    { 57, 8, "Folded clothes disappear quickly into these deep Dresser drawers. Even full, they roll smoothly. The top still holds a mirror and lamp.", ReviewDate(312, 9, 24), 12, 4 },
                    { 58, 9, "Two drawer fronts on the Dresser needed adjustment before they sat flush. It is serviceable afterward, although that step was more fiddly than it should be.", ReviewDate(318, 16, 49), 12, 3 },
                    { 59, 10, "Delivery left the Dresser right at our door. It feels heavy in the reassuring way, and opening several drawers at once has not made the frame rack.", ReviewDate(324, 12, 12), 12, 5 },
                    { 60, 11, "I feared the Dresser would overwhelm the bedroom. Measured against the wall, it fits well, and the handles are comfortable even when a drawer is full.", ReviewDate(330, 18, 34), 12, 4 },
                    { 61, 12, "Drawer-runner instructions could be clearer. After that frustrating part, the Dresser proved solid, attractive, and much more useful than its plain photo.", ReviewDate(336, 10, 56), 12, 4 },
                    { 62, 13, "Useful Dresser, but its shallow bottom drawer keeps this at three stars.", ReviewDate(342, 21, 3), 12, 3 },
                    { 63, 1, "A deep seat without a giant footprint is hard to find. The Armchair fills our reading corner. Its cushions bounce back overnight.", ReviewDate(348, 8, 27), 13, 5 },
                    { 64, 2, "Reading feels easier with the supportive arms on this Armchair. Note that its color is warmer than it appeared on my monitor.", ReviewDate(354, 15, 51), 13, 4 },
                    { 65, 4, "The Armchair sits level and balanced, though I wish its seat were softer.", ReviewDate(360, 19, 14), 13, 4 },
                    { 66, 5, "Beautiful silhouette, frustrating back cushion. During a longer sit on the Armchair it slowly slides down, though it still works well as an accent piece.", ReviewDate(366, 11, 38), 13, 3 },
                    { 67, 6, "No odor on opening the box, and the Armchair fits the measured space exactly. It has enough weight to feel dependable for everyday use.", ReviewDate(372, 20, 1), 13, 4 },
                    { 68, 7, "At our island, the Bar Stool height and footrest both land where they should. The included wrench was enough. Assembly took very little time.", ReviewDate(378, 9, 32), 14, 4 },
                    { 69, 8, "Looks good beside the counter, but this Bar Stool is too firm for lingering over coffee. Its base stays steady and has not marked the floor.", ReviewDate(384, 16, 55), 14, 3 },
                    { 70, 9, "Spills wipe off easily and the Bar Stool swivels without a scrape. It feels much more substantial than the price implied.", ReviewDate(390, 12, 18), 14, 5 },
                    { 71, 10, "One leg had a tiny mark, easily hidden toward the wall. The Bar Stool adjusts height reliably, so I did not bother chasing a replacement.", ReviewDate(396, 18, 43), 14, 4 },
                    { 72, 11, "Tight bolt holes turned building the Bar Stool into an annoyance. Finished product is fine for quick meals, but not worth the trouble at this price.", ReviewDate(402, 10, 7), 14, 2 },
                    { 73, 12, "Narrow-kitchen friendly Bar Stool with a clean profile and well-placed footrest.", ReviewDate(408, 21, 25), 14, 4 },
                    { 74, 13, "Serving dishes used to live in three places. The Sideboard gathers them behind doors that close cleanly. Its wood tone works beside our older table.", ReviewDate(414, 8, 48), 15, 5 },
                    { 75, 1, "Door alignment needed patience during Sideboard assembly. Once adjusted, the cabinet feels sturdy and the shelves have been genuinely useful.", ReviewDate(420, 15, 11), 15, 4 },
                    { 76, 2, "From across the room the Sideboard looks lovely. Up close, its drawer bottoms seem too light for heavy dishes, so ours hold linens instead.", ReviewDate(426, 19, 36), 15, 3 },
                    { 77, 4, "I measured twice before ordering the Sideboard, and it fits the dining wall exactly. After meals, the top wipes clean with almost no effort.", ReviewDate(432, 11, 58), 15, 4 },
                    { 78, 5, "No damage on delivery, pleasing weight after assembly, and lots of storage without a bulky look. The Sideboard has been a very good addition.", ReviewDate(438, 20, 14), 15, 5 },
                    { 79, 6, "Two adults fit on the Bench, which stays planted on hardwood after a simple build.", ReviewDate(444, 9, 41), 16, 4 },
                    { 80, 7, "Useful landing spot at the entry. The Bench frame feels dependable. Its finish picked up a scuff sooner than I hoped.", ReviewDate(450, 16, 6), 16, 3 },
                    { 81, 8, "Under the window, the Bench fits the listed depth perfectly. It is sturdier than it looks, and the cushion has enough give for a long phone call.", ReviewDate(456, 12, 31), 16, 5 },
                    { 82, 9, "Clean lines were the reason I chose this Bench. The separated hardware made setup painless, and it is comfortable for putting on shoes without lingering.", ReviewDate(462, 18, 54), 16, 4 },
                    { 83, 10, "Late delivery and one rough stain patch spoiled the first impression. Our Bench is usable, but quality control should have caught that leg.", ReviewDate(468, 10, 25), 16, 3 },
                    { 84, 11, "Easy-to-carry Bench that feels solid and fairly priced.", ReviewDate(474, 21, 9), 16, 4 },
                    { 85, 12, "Nearly assembled from the box, the Rocking Chair started earning its keep immediately. The motion is quiet. Evening reading has become a ritual.", ReviewDate(480, 8, 36), 17, 5 },
                    { 86, 13, "Comfortable Rocking Chair, though its armrests could be wider.", ReviewDate(486, 15, 57), 17, 4 },
                    { 87, 1, "Before ordering, check the listed seat height; this Rocking Chair sits lower than ours. It rocks steadily, though I added a small cushion.", ReviewDate(492, 19, 21), 17, 3 },
                    { 88, 2, "Every evening I use the Rocking Chair by the window. The runners have not marked the floor, and the joints still feel tight.", ReviewDate(498, 11, 44), 17, 4 },
                    { 89, 4, "Careful packing left our Rocking Chair free of chips and loose hardware. It is comfortable enough that the price now seems reasonable.", ReviewDate(504, 20, 6), 17, 5 },
                    { 90, 5, "Lovely Rocking Chair; its floor pads failed quickly, but the chair remains stable.", ReviewDate(510, 9, 29), 17, 4 },
                    { 91, 6, "Cleaning supplies finally live behind closed doors. The Storage Cabinet fits our utility room. Its doors do not drift open.", ReviewDate(516, 16, 53), 18, 4 },
                    { 92, 7, "Several nearly identical panels made the Storage Cabinet slower to assemble than expected. It is useful once built, but the shelves are light duty.", ReviewDate(522, 12, 16), 18, 3 },
                    { 93, 8, "Tall bottles fit after I moved the adjustable Storage Cabinet shelves. Anchored to the wall, the whole unit feels very secure.", ReviewDate(528, 18, 38), 18, 5 },
                    { 94, 9, "The narrow Storage Cabinet depth suits our tight laundry room, and damp towels have not swollen its finish.", ReviewDate(534, 10, 2), 18, 4 },
                    { 95, 10, "One Storage Cabinet shelf arrived dented and the replacement took too long. The cabinet is sturdy otherwise, but the delay was frustrating.", ReviewDate(540, 21, 19), 18, 3 },
                    { 96, 11, "More usable room than its footprint suggests. This Storage Cabinet has aligned doors, decent hardware, and a price that made sense for our laundry room.", ReviewDate(546, 8, 47), 18, 4 }
                });

            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Users\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Users\"), 1), true);");
            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Reviews\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Reviews\"), 1), true);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
                    25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44,
                    45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64,
                    65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84,
                    85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96
                });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValues: new object[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 });

            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Users\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Users\"), 1), true);");
            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Reviews\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Reviews\"), 1), true);");
        }

        private static DateTimeOffset ReviewDate(int days, int hour, int minute)
            => new(new DateTime(2025, 1, 2, hour, minute, 0, DateTimeKind.Utc).AddDays(days));
    }
}

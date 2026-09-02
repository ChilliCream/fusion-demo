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
                    { 5, 4, "After two dinners on it, I am pleased with the table. Nothing shifts when someone leans on an edge. Even the matte top ignores water rings.", ReviewDate(588, 9, 17), 1, 4 },
                    { 6, 5, "The breakfast nook is tight, but this table leaves enough space to pull out every chair. Assembly was painless because the bolts and washers came in separate, clearly marked bags.", ReviewDate(43, 18, 42), 1, 4 },
                    { 7, 6, "Heavy legs, no wobble: this table handles workdays and dinners beautifully.", ReviewDate(273, 11, 8), 1, 5 },
                    { 8, 7, "Seven days in, the initially firm cushions on our new couch finally relaxed. Delivery staff carried it upstairs carefully. The seat depth is exactly right for stretching out.", ReviewDate(601, 14, 31), 2, 5 },
                    { 9, 8, "Two cats have tested the fabric harder than we have. The sofa has not snagged, the low back is comfortable, and it feels worth the higher price.", ReviewDate(112, 20, 5), 2, 5 },
                    { 10, 9, "For quick dinners this chair is fine. The compact shape saves room, but the firm seat sends me looking for a cushion after an hour.", ReviewDate(522, 8, 54), 3, 3 },
                    { 11, 10, "Under my desk, the chair disappears neatly. Its finish matched the photograph better than expected, and there has been no creak from the frame.", ReviewDate(16, 16, 19), 3, 4 },
                    { 12, 11, "A comfortable chair with good lumbar support, tidy screws, and real value.", ReviewDate(311, 12, 47), 3, 5 },
                    { 13, 12, "Uneven floors usually make shelving a headache. This bookcase leveled easily. Its dark finish gives the room a calmer backdrop for books.", ReviewDate(31, 19, 26), 4, 5 },
                    { 14, 13, "Paperbacks fill the bookcase without any visible sagging. It is slimmer than my previous unit, while the back panel feels a little too light.", ReviewDate(476, 10, 13), 4, 4 },
                    { 15, 1, "One drawing in the instructions sent me backward, so assembling the shelves took longer than planned. The bookcase stands steady now, although the manual needs work.", ReviewDate(204, 15, 38), 4, 3 },
                    { 16, 2, "Taller art books finally have a home. We anchored the bookcase, moved the adjustable shelves twice, and ended up with a very secure unit.", ReviewDate(590, 21, 7), 4, 5 },
                    { 17, 4, "No dents, no chemical smell, no surprise. The bookcase arrived in a tall box and the finish is pleasantly smooth rather than plastic-looking.", ReviewDate(133, 9, 45), 4, 4 },
                    { 18, 5, "This bookcase stays square under cookbooks and costs less than comparable pieces.", ReviewDate(405, 17, 22), 4, 4 },
                    { 19, 6, "My home office needed room for two monitors, not another giant surface. The desk strikes that balance. Threading cables underneath made the setup look finished.", ReviewDate(560, 13, 56), 5, 5 },
                    { 20, 7, "Drawer slides are quiet and the desk feels solid. In a small office, the stated depth leaves a workable path behind my chair.", ReviewDate(72, 20, 11), 5, 4 },
                    { 21, 8, "Once assembled, the desk looked better than the photos. One predrilled hole was a little tight; five minutes with a file fixed it, and everything has stayed level.", ReviewDate(381, 8, 33), 5, 4 },
                    { 22, 9, "The drawer diagram is the weak point here. Once I got past that, the desk worked well enough, though its thin back panel feels cheaper than the rest.", ReviewDate(245, 16, 58), 5, 3 },
                    { 23, 10, "It reached us ahead of schedule with every corner protected. This desk is a practical, scratch-free workspace at a price I can live with.", ReviewDate(0, 11, 24), 5, 5 },
                    { 24, 11, "Our heavy mattress has not produced a single squeak from the bed frame. The slats clicked into place. Getting out of bed no longer wakes the other person.", ReviewDate(156, 18, 49), 6, 5 },
                    { 25, 12, "Getting the bed frame upstairs took two people. Assembly afterward was easy, and the upholstered edge is nicer on bare shins than I expected.", ReviewDate(597, 9, 6), 6, 4 },
                    { 26, 13, "The bed frame feels properly braced without a box spring, despite unhelpful rail labels.", ReviewDate(27, 14, 42), 6, 4 },
                    { 27, 1, "A scuffed corner arrived on our bed frame, but it faces the wall. More importantly, it has stayed quiet and sturdy through several restless weeks.", ReviewDate(426, 21, 15), 6, 3 },
                    { 28, 2, "Clean lines and useful clearance underneath made this bed frame work in a tight room. The center support feels reassuring with a larger mattress.", ReviewDate(303, 10, 27), 6, 4 },
                    { 29, 4, "I built the bed frame solo before lunch. The even finish and lack of movement when I sit down make it one of the better flat-pack purchases here.", ReviewDate(84, 17, 53), 6, 5 },
                    { 30, 5, "Chargers, a paperback, and reading glasses all fit in the drawer. The nightstand lines up neatly with our mattress. That matters more than I expected.", ReviewDate(508, 8, 16), 7, 4 },
                    { 31, 6, "Nice grain and no sharp corners on the nightstand. The drawer catches in the final inch, which is irritating but not return-worthy.", ReviewDate(64, 15, 41), 7, 3 },
                    { 32, 7, "Mostly assembled on arrival, our nightstand felt heavier than expected. Beside the bed, it looks far less like flat-pack furniture than it is.", ReviewDate(595, 19, 9), 7, 5 },
                    { 33, 8, "That awkward sliver beside the bed finally has a purpose. I measured first; the bedside table fits exactly, and the open shelf catches my glasses before they vanish.", ReviewDate(219, 11, 34), 7, 4 },
                    { 34, 9, "The nightstand arrived intact, stable, and reasonably priced.", ReviewDate(341, 20, 2), 7, 4 },
                    { 35, 10, "Board-game night is less chaotic now. Snacks wipe straight off the coffee table. Controllers and coasters disappear onto the lower shelf.", ReviewDate(18, 9, 28), 8, 5 },
                    { 36, 11, "Aligning one shelf took a second attempt, but the coffee table became sturdy afterward. Its proportions suit the sofa instead of crowding the rug.", ReviewDate(444, 16, 52), 8, 4 },
                    { 37, 12, "A chipped corner greeted me when I opened the carton, and the coating on the coffee table already looks patchy. At this price I expected much better quality.", ReviewDate(278, 12, 14), 8, 2 },
                    { 38, 13, "Rounded corners mattered most with a toddler learning to walk. The coffee table is heavy enough to stay put when little hands use it for balance.", ReviewDate(604, 18, 37), 8, 5 },
                    { 39, 1, "Check the height before ordering: our coffee table sits lower than I pictured. The wood tone is beautiful, so we kept it.", ReviewDate(101, 10, 59), 8, 4 },
                    { 40, 2, "An easy evening build and a month of daily use later, the coffee table still feels solid. Sensible value without trying to look fancy.", ReviewDate(366, 21, 21), 8, 4 },
                    { 41, 4, "Long meals are easier in this dining chair than in our old set. The curved back supports without pressing into my shoulders. Every leg sits flat on tile.", ReviewDate(535, 8, 43), 9, 4 },
                    { 42, 5, "Nice dining chair, but the thin padding limits dinner to an hour.", ReviewDate(6, 15, 5), 9, 3 },
                    { 43, 6, "Spilled sauce wiped off the fabric on the dining chair without leaving a mark. We ordered several, and the stitching and color are remarkably consistent across all of them.", ReviewDate(392, 19, 32), 9, 5 },
                    { 44, 7, "Everything needed for assembly was sorted into labeled packets. The dining chair sits at a comfortable height beside our table and feels strong once the bolts are tightened.", ReviewDate(164, 11, 57), 9, 4 },
                    { 45, 8, "For a small dining room, the chair has the right footprint. Straightforward setup and a price low enough that I would order another.", ReviewDate(601, 20, 18), 9, 4 },
                    { 46, 9, "I got useful hanging room from the wardrobe, but its shelves are shallower than I expected. Budget an afternoon for assembly and do not try it alone.", ReviewDate(250, 9, 39), 10, 3 },
                    { 47, 10, "An open rack made our bedroom look unfinished. This wardrobe closes evenly, fits below the ceiling by a sensible margin, and tidies the room immediately.", ReviewDate(587, 16, 4), 10, 4 },
                    { 48, 11, "The finished wardrobe does not sway, which matters because the panels are hefty. Attaching the doors really needs a second person.", ReviewDate(14, 12, 29), 10, 3 },
                    { 49, 12, "Quiet hinges won me over. With the rail adjusted, the wardrobe holds dresses and shirts neatly, and its white finish wipes clean after dusty weeks.", ReviewDate(417, 18, 51), 10, 4 },
                    { 50, 13, "All the hardware and panels were labeled, and nothing arrived damaged. The wardrobe is plain, but it closes neatly and keeps the bedroom from looking cluttered.", ReviewDate(329, 10, 22), 10, 3 },
                    { 51, 1, "Need closed storage quickly? This wardrobe does the job. The drawers are smaller than anticipated, yet the cabinet itself feels stable and fairly priced.", ReviewDate(92, 21, 8), 10, 4 },
                    { 52, 2, "Cables no longer spill out behind the television. The TV console has room for our soundbar and game system, with useful openings exactly where they are needed.", ReviewDate(472, 8, 35), 11, 5 },
                    { 53, 4, "Leveling took minutes, then the doors on the media console lined up cleanly. It sits lower than our old stand, just as the listed height warned.", ReviewDate(39, 15, 54), 11, 4 },
                    { 54, 5, "Game console stays cool. Shelves feel firm. This TV stand was painless to assemble.", ReviewDate(600, 19, 17), 11, 4 },
                    { 55, 6, "We like the finish on the TV stand, but the supplied wall straps were too short for our setup. The cabinet stands solidly without them.", ReviewDate(186, 11, 46), 11, 3 },
                    { 56, 7, "Well packed and better-looking than its price suggested, the media stand hides remotes and cables beautifully. Guests assume it cost more.", ReviewDate(348, 20, 9), 11, 5 },
                    { 57, 8, "Folded clothes disappear quickly into the deep drawers in this dresser. Even full, they roll smoothly. The top still holds a mirror and lamp.", ReviewDate(9, 9, 24), 12, 4 },
                    { 58, 9, "Two drawer fronts on the dresser needed adjustment before they sat flush. It is serviceable afterward, although that step was more fiddly than it should be.", ReviewDate(519, 16, 49), 12, 3 },
                    { 59, 10, "Delivery left the dresser right at our door. It feels heavy in the reassuring way, and opening several drawers at once has not made the frame rack.", ReviewDate(225, 12, 12), 12, 5 },
                    { 60, 11, "I feared the dresser would overwhelm the bedroom. Measured against the wall, it fits well, and the handles are comfortable even when a drawer is full.", ReviewDate(588, 18, 34), 12, 4 },
                    { 61, 12, "Drawer-runner instructions could be clearer. After that frustrating part, the dresser proved solid, attractive, and much more useful than its plain photo.", ReviewDate(118, 10, 56), 12, 4 },
                    { 62, 13, "Useful dresser, but its shallow bottom drawer keeps this at three stars.", ReviewDate(401, 21, 3), 12, 3 },
                    { 63, 1, "A deep seat without a giant footprint is hard to find. The armchair fills our reading corner. Its cushions bounce back overnight.", ReviewDate(548, 8, 27), 13, 5 },
                    { 64, 2, "Reading feels easier with the supportive arms on this armchair. Note that its color is warmer than it appeared on my monitor.", ReviewDate(55, 15, 51), 13, 4 },
                    { 65, 4, "The armchair sits level and balanced, though I wish its seat were softer.", ReviewDate(370, 19, 14), 13, 4 },
                    { 66, 5, "Beautiful silhouette, frustrating back cushion. During a longer sit in the armchair it slowly slides down, though the chair still works well as an accent piece.", ReviewDate(602, 11, 38), 13, 3 },
                    { 67, 6, "No odor when I opened it, and the armchair fits the measured space exactly. It has enough weight to feel dependable for everyday use.", ReviewDate(196, 20, 1), 13, 4 },
                    { 68, 7, "At our island, the height of the bar stool and its footrest both land where they should. The included wrench was enough. Assembly took very little time.", ReviewDate(23, 9, 32), 14, 4 },
                    { 69, 8, "Looks good beside the counter, but this stool is too firm for lingering over coffee. Its base stays steady and has left no marks on the floor.", ReviewDate(459, 16, 55), 14, 3 },
                    { 70, 9, "Spills wipe off easily and the bar stool swivels without a scrape. It feels much more substantial than the price implied.", ReviewDate(297, 12, 18), 14, 5 },
                    { 71, 10, "One leg had a tiny mark, easily hidden toward the wall. The stool adjusts height reliably, so I did not bother chasing a replacement.", ReviewDate(581, 18, 43), 14, 4 },
                    { 72, 11, "Tight bolt holes turned building the bar stool into an annoyance. The finished seat is fine for quick meals, but not worth the trouble at this price.", ReviewDate(129, 10, 7), 14, 2 },
                    { 73, 12, "A narrow-kitchen friendly stool with a clean profile and well-placed footrest.", ReviewDate(402, 21, 25), 14, 4 },
                    { 74, 13, "Serving dishes used to live in three places. The sideboard gathers them behind doors that close cleanly. Its wood tone works beside our older table.", ReviewDate(575, 8, 48), 15, 5 },
                    { 75, 1, "Getting both doors even took patience and several small adjustments. After that, the sideboard felt sturdy, and its shelves have been useful for serving bowls and linens.", ReviewDate(87, 15, 11), 15, 4 },
                    { 76, 2, "From across the room the sideboard looks lovely. Up close, its drawer bottoms seem too light for heavy dishes, so ours hold linens instead.", ReviewDate(432, 19, 36), 15, 3 },
                    { 77, 4, "I measured twice before ordering the sideboard, and it fits the dining wall exactly. After meals, the top wipes clean with almost no effort.", ReviewDate(11, 11, 58), 15, 4 },
                    { 78, 5, "No damage on delivery, pleasing weight after assembly, and lots of storage without a bulky look. The sideboard has been a very good addition.", ReviewDate(306, 20, 14), 15, 5 },
                    { 79, 6, "Two adults fit on the bench, which stays planted on hardwood after a simple build.", ReviewDate(144, 9, 41), 16, 4 },
                    { 80, 7, "Useful landing spot at the entry. The frame of the bench feels dependable. Its finish picked up a scuff sooner than I hoped.", ReviewDate(599, 16, 6), 16, 3 },
                    { 81, 8, "Under the window, the bench fits the listed depth perfectly. It is sturdier than it looks, and the cushion has enough give for a long phone call.", ReviewDate(40, 12, 31), 16, 5 },
                    { 82, 9, "Clean lines were the reason I chose this bench. The separated hardware made setup painless, and it is comfortable for putting on shoes without lingering.", ReviewDate(483, 18, 54), 16, 4 },
                    { 83, 10, "Late delivery and one rough stain patch spoiled the first impression. Our bench is usable, but quality control should have caught that leg.", ReviewDate(263, 10, 25), 16, 3 },
                    { 84, 11, "An easy-to-carry bench that feels solid and fairly priced.", ReviewDate(352, 21, 9), 16, 4 },
                    { 85, 12, "Nearly assembled from the box, the rocking chair started earning its keep immediately. The motion is quiet. Evening reading has become a ritual.", ReviewDate(592, 8, 36), 17, 5 },
                    { 86, 13, "A comfortable rocking chair, though its armrests could be wider.", ReviewDate(76, 15, 57), 17, 4 },
                    { 87, 1, "Before ordering, check the listed seat height; this rocking chair sits lower than ours. It rocks steadily, though I added a small cushion.", ReviewDate(413, 19, 21), 17, 3 },
                    { 88, 2, "Every evening I use the rocking chair by the window. The runners have not marked the floor, and the joints still feel tight.", ReviewDate(230, 11, 44), 17, 4 },
                    { 89, 4, "Careful packing left our rocking chair free of chips and loose hardware. It is comfortable enough that the price now seems reasonable.", ReviewDate(3, 20, 6), 17, 5 },
                    { 90, 5, "A lovely rocking chair; its floor pads failed quickly, but the chair remains stable.", ReviewDate(506, 9, 29), 17, 4 },
                    { 91, 6, "Cleaning supplies finally live behind closed doors. The storage cabinet fits our utility room. Its doors do not drift open.", ReviewDate(126, 16, 53), 18, 4 },
                    { 92, 7, "Several nearly identical panels made the storage cabinet slower to assemble than expected. It is useful once built, but the shelves are light duty.", ReviewDate(603, 12, 16), 18, 3 },
                    { 93, 8, "I rearranged the shelves to make space for tall detergent bottles. With the cabinet anchored to the wall, it feels exceptionally solid and safe.", ReviewDate(47, 18, 38), 18, 5 },
                    { 94, 9, "Our laundry room has almost no spare floor space, so the cabinet's shallow depth is ideal. Damp towels have brushed against the finish for months without making it swell.", ReviewDate(451, 10, 2), 18, 4 },
                    { 95, 10, "One shelf in the storage cabinet arrived dented and the replacement took too long. The rest is sturdy, but the delay was frustrating.", ReviewDate(289, 21, 19), 18, 3 },
                    { 96, 11, "More usable room than its footprint suggests. This utility cabinet has aligned doors, decent hardware, and a price that made sense for our laundry room.", ReviewDate(544, 8, 47), 18, 4 }
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

<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="JobListingResult.aspx.cs" Inherits="NEWWW.JobListingResult" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

 <div class="search-section">
  <div>
   <div class="search-box">
    <input type="text" placeholder="Enter keywords" />
    <select>
     <option>Any classification</option>
    </select>
    <input type="text" placeholder="Enter suburb, city, or region" />
   </div>
   <div class="more-options">
    <button class="search-btn" onclick="window.location = 'jobsearch.html';">SEEK</button>
    More options <span>⚙️</span></div>
  </div>
 </div>
 <!-- use careers sg search results -->
 <!-- not empty search, unhide the job n replace words, still has filters -->
 <!-- empty will still show the filters with a pls search somemthing -->
 <div class="jobsearch-container">
  <!-- Filters Section -->
  <div class="filters">
   <div class="filter-section">
    <label for="sort-by">Sort By:</label>
    <select id="sort-by">
     <option value="relevance">Relevance</option>
     <option value="date">Date</option>
    </select>
   </div>
 
   <div class="filter-section">
    <label for="salary">Minimum Salary:</label>
    <input type="number" id="salary" placeholder="Enter salary" />
   </div>
 
   <div class="filter-section">
    <label for="employment-type">Employment Type:</label>
    <select id="employment-type">
     <option value="part-time">Part Time</option>
     <option value="full-time">Full Time</option>
    </select>
   </div>
 
   <div class="filter-section">
    <label for="job-function">Job Function:</label>
    <select id="job-function">
     <option value="information-technology">Information Technology</option>
     <option value="marketing">Marketing</option>
    </select>
   </div>
 
   <div class="filter-section">
    <label for="gov-support">Government Support:</label>
    <div>
     <input type="checkbox" id="gov-support" />
     <label for="gov-support">For Fresh Graduates</label>
    </div>
    <div>
     <input type="checkbox" id="mid-career" />
     <label for="mid-career">For Mid-career Attachments</label>
    </div>
   </div>

   <div class="filter-section">
    <h3>Flexible Work Arrangements (FWA)</h3>
    <div class="filter-checkboxes">
     <div><input type="checkbox" id="flexi-hours"> <label for="flexi-hours">Flexi-Hours</label></div>
     <div><input type="checkbox" id="telecommuting"> <label for="telecommuting">Telecommuting</label></div>
     <div><input type="checkbox" id="choice-days-off"> <label for="choice-days-off">Employees Choice of Days Off</label>
     </div>
     <div><input type="checkbox" id="staggered-time"> <label for="staggered-time">Staggered Time</label></div>
     <div><input type="checkbox" id="compressed-schedule"> <label for="compressed-schedule">Compressed Work
       Schedule</label></div>
     <div><input type="checkbox" id="creative-scheduling"> <label for="creative-scheduling">Creative Scheduling</label>
     </div>
    </div>
    <p><a href="#" class="filter-link">Learn More About FWAs</a></p>
   </div>
   
   <div class="filter-section">
    <h3>Location</h3>
    <div class="filter-checkboxes">
     <div><input type="checkbox" id="central"> <label for="central">Central</label></div>
     <div><input type="checkbox" id="east"> <label for="east">East</label></div>
     <div><input type="checkbox" id="north"> <label for="north">North</label></div>
     <div><input type="checkbox" id="south"> <label for="south">South</label></div>
     <div><input type="checkbox" id="west"> <label for="west">West</label></div>
     <div><input type="checkbox" id="islandwide"> <label for="islandwide">Islandwide</label></div>
    </div>
   </div>
   
   <div class="filter-section">
    <h3>Job Level</h3>
    <div class="filter-checkboxes">
     <div><input type="checkbox" id="senior-management"> <label for="senior-management">Senior Management</label></div>
     <div><input type="checkbox" id="middle-management"> <label for="middle-management">Middle Management</label></div>
     <div><input type="checkbox" id="manager"> <label for="manager">Manager</label></div>
     <div><input type="checkbox" id="professional"> <label for="professional">Professional</label></div>
     <div><input type="checkbox" id="senior-executive"> <label for="senior-executive">Senior Executive</label></div>
     <div><input type="checkbox" id="executive"> <label for="executive">Executive</label></div>
     <div><input type="checkbox" id="junior-executive"> <label for="junior-executive">Junior Executive</label></div>
     <div><input type="checkbox" id="non-executive"> <label for="non-executive">Non-executive</label></div>
     <div><input type="checkbox" id="entry-level"> <label for="entry-level">Fresh/Entry Level</label></div>
    </div>
   </div>

  </div>

  
 
  <!-- Search Results Section -->
  <div class="results">
   <div class="job-alert">
    <p>Want to receive email alerts on new jobs similar to those shown below?</p>
    <button onclick="window.location = '../login.html';">Log in to Create Job Alert</button>
   </div>
 
   <div class="gov-support">
    <p>Thinking of a new career? If you are a mid-career individual aged 40 and above, explore attachment opportunities
     with host organisations to gain industry-relevant skills and receive a training allowance.</p>
   </div>
 
    <!--here is where i should display my listing data / api data-->

   <div class="job-list">

    <div class="job-item">
     <div>
      <h3>Software Developer (Learning Sciences & Assessment) [NIE]</h3>
      <p><strong>Location:</strong> West</p>
      <p><strong>Employment Type:</strong> Part Time</p>
      <p><strong>Job Function:</strong> Information Technology</p>
      <p class="salary">$1,350 to $1,850 Monthly</p>
      <p>0% skills matched</p>
     </div>
     <div>
      <button>Apply</button>
      <p>Posted Yesterday</p>
     </div>
    </div>
 
    <div class="job-item">
     <div>
      <h3>Web Systems and Platform Support (Intern / Part-Time)</h3>
      <p><strong>Location:</strong> West</p>
      <p><strong>Employment Type:</strong> Part Time</p>
      <p><strong>Job Function:</strong> Information Technology</p>
      <p class="salary">$500 to $1,500 Monthly</p>
      <p>0% skills matched</p>
     </div>
     <div>
      <button>Apply</button>
      <p>Posted 8 days ago</p>
     </div>
    </div>

    <!-- here is how we display the job post using the metadata, using a repeater template -->
    <asp:Repeater ID="rptJobs" runat="server" OnItemCommand="rptJobs_ItemCommand">
        <ItemTemplate>
            

            <div class="job-item">
                <div>
                    <h3><%# Eval("job_title") %></h3>
                    <p><strong>Location:</strong> <%# Eval("location") %></p>
                    <p><strong>Company:</strong> <%# Eval("company") %></p>
                    <p class="salary"><%# Eval("salary_string") %></p>
                    <p><strong>Main URL:</strong> <%# Eval("source_url") %></p>
                    <p>0% skills matched</p>
                </div>
                <div>
                    <asp:LinkButton runat="server" Text="Apply" CommandName="Apply"
                    CommandArgument='<%# Eval("id") %>' />

                    <p>Date Posted: <%# Eval("date_posted", "{0:MM/dd/yyyy}") %></p>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

   </div>
  
  </div>


 </div>

</asp:Content>

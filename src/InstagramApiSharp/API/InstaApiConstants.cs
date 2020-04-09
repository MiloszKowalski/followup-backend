/*  
 *  
 *  
 *  All endpoints and headers is here
 *  
 *  
 *        IRANIAN DEVELOPERS
 *        
 *        
 *               2019
 *  
 *  
 */

using Newtonsoft.Json.Linq;
using System;

namespace InstagramApiSharp.API
{
    /// <summary>
    ///     Place of every endpoints, headers and other constants and variables.
    /// </summary>
    internal static class InstaApiConstants
    {
        #region New

        public const string BANYAN = API_SUFFIX + "/banyan/banyan//?views=[\"group_stories_share_sheet\",\"reshare_share_sheet\",\"story_share_sheet\",\"threads_people_picker\"]";
        public const string FBSEARCH_DYNAMIC_SEARCH = API_SUFFIX + "/fbsearch/nullstate_dynamic_sections/?type={0}";
        public const string STORY_QUIZ_ANSWER = API_SUFFIX + "/media/{0}/{1}/story_quiz_answer/";

        public const string ACCOUNTS_GET_PREFILL_CANDIDATES = API_SUFFIX + "/accounts/get_prefill_candidates/";
        public const string COMMERCE_BAG_COUNT = API_SUFFIX + "/commerce/bag/count/";
        public const string PROFILE_SU_BADGE = API_SUFFIX + "/discover/profile_su_badge/";
        public const string MARK_SU_SEEN = API_SUFFIX + "/discover/mark_su_seen/";
        public const string PROFILE_ARCHIVE_BADGE = API_SUFFIX + "/archive/reel/profile_archive_badge/";
        public const string MULTIPLE_ACCOUNTS_GET_ACCOUNT_FAMILY = API_SUFFIX + "/multiple_accounts/get_account_family/";
        public const string QP_GET_COOLDOWNS = API_SUFFIX + "/qp/get_cooldowns/?signed_body=9b73453b2bf2508dc3d3161cbb5ef33296d31052b292dd3795d3bcea8b0b8dc5.%7B%7D&ig_sig_key_version=4";
        public const string USER_SCORES = API_SUFFIX + "/scores/bootstrap/users/?surfaces=[\"coefficient_rank_recipient_user_suggestion\",\"coefficient_besties_list_ranking\",\"coefficient_direct_recipients_ranking_variant_2\",\"coefficient_ios_section_test_bootstrap_ranking\",\"autocomplete_user_list\"]";
        public const string GET_LINKAGE_STATUS = API_SUFFIX + "/linked_accounts/get_linkage_status/";
        public const string LOOM_FETCH_CONFIG = API_SUFFIX + "/loom/fetch_config/";
        public const string QE_SYNC = API_SUFFIX + "/qe/sync/";
        public const string BUSINESS_ELIGIBILITY = API_SUFFIX + "/business/eligibility/get_monetization_products_eligibility_data/?product_types=branded_content";
        public const string BUSINESS_BRANDED_CONTENT = API_SUFFIX + "/business/branded_content/should_require_professional_account/";

        #region QpBatchFetch

        public const string QP_BATCH_FETCH = API_SUFFIX + "/qp/batch_fetch/";
        public const string SURFACES_TO_TRIGGERS_OTHER_LOGGED_IN_USER = "{\"4715\":[\"instagram_other_logged_in_user_id_loaded\"],\"5858\":[],\"5734\":[]}";
        public const string SURFACES_TO_QUERIES_OTHER_LOGGED_IN_USER = "{\"4715\":\"Query QuickPromotionSurfaceQuery: Viewer {viewer() {eligible_promotions.trigger_context_v2(<trigger_context_v2>).ig_parameters(<ig_parameters>).trigger_name(<trigger_name>).surface_nux_id(<surface>).external_gating_permitted_qps(<external_gatin" +
            "g_permitted_qps>).supports_client_filters(true).include_holdouts(true) {edges {client_ttl_seconds,log_eligibility_waterfall,is_holdout,priority,time_range {start,end},node {id,promotion_id,logging_data,max_impressions,triggers,contextual_filters {clause_type,filters {filter_type,unknown_action" +
            ",value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}" +
            "},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},ext" +
            "ra_datas {name,required,bool_value,int_value,string_value}}}}}},is_uncancelable,template {name,parameters {name,required,bool_value,string_value,color_value,}},creatives {title {text},content {text},footer {text},social_context {text},social_context_images,primary_action{title {text},url,limit" +
            ",dismiss_promotion},secondary_action{title {text},url,limit,dismiss_promotion},dismiss_action{title {text},url,limit,dismiss_promotion},image.scale(<scale>) {uri,width,height}}}}}}}\",\"5858\":\"Query QuickPromotionSurfaceQuery: Viewer {viewer() {eligible_promotions.trigger_context_v2(<trigger" +
            "_context_v2>).ig_parameters(<ig_parameters>).trigger_name(<trigger_name>).surface_nux_id(<surface>).external_gating_permitted_qps(<external_gating_permitted_qps>).supports_client_filters(true).include_holdouts(true) {edges {client_ttl_seconds,log_eligibility_waterfall,is_holdout,priority,time_" +
            "range {start,end},node {id,promotion_id,logging_data,max_impressions,triggers,contextual_filters {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters " +
            "{filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_va" +
            "lue,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}}}}}},is_uncancelable,template {name,parameters {name,required,bool_value,string_value,colo" +
            "r_value,}},creatives {title {text},content {text},footer {text},social_context {text},social_context_images,primary_action{title {text},url,limit,dismiss_promotion},secondary_action{title {text},url,limit,dismiss_promotion},dismiss_action{title {text},url,limit,dismiss_promotion},image.scale(<" +
            "scale>) {uri,width,height}}}}}}}\",\"5734\":\"Query QuickPromotionSurfaceQuery: Viewer {viewer() {eligible_promotions.trigger_context_v2(<trigger_context_v2>).ig_parameters(<ig_parameters>).trigger_name(<trigger_name>).surface_nux_id(<surface>).external_gating_permitted_qps(<external_gating_pe" +
            "rmitted_qps>).supports_client_filters(true).include_holdouts(true) {edges {client_ttl_seconds,log_eligibility_waterfall,is_holdout,priority,time_range {start,end},node {id,promotion_id,logging_data,max_impressions,triggers,contextual_filters {clause_type,filters {filter_type,unknown_action,val" +
            "ue {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},cl" +
            "auses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_d" +
            "atas {name,required,bool_value,int_value,string_value}}}}}},is_uncancelable,template {name,parameters {name,required,bool_value,string_value,color_value,}},creatives {title {text},content {text},footer {text},social_context {text},social_context_images,primary_action{title {text},url,limit,dis" +
            "miss_promotion},secondary_action{title {text},url,limit,dismiss_promotion},dismiss_action{title {text},url,limit,dismiss_promotion},image.scale(<scale>) {uri,width,height}}}}}}}\"}";

        public const string SURFACES_TO_TRIGGERS_EXPLORE_HEADER = "{\"4715\":[\"instagram_explore_header\"],\"5734\":[\"instagram_explore_prompt\"]}";
        public const string SURFACES_TO_QUERIES_EXPLORE_HEADER = "{\"4715\":\"Query QuickPromotionSurfaceQuery: Viewer {viewer() {eligible_promotions.trigger_context_v2(<trigger_context_v2>).ig_parameters(<ig_parameters>).trigger_name(<trigger_name>).surface_nux_id(<surface>).external_gating_permitted_qps(" +
            "<external_gating_permitted_qps>).supports_client_filters(true).include_holdouts(true) {edges {client_ttl_seconds,log_eligibility_waterfall,is_holdout,priority,time_range {start,end},node {id,promotion_id,logging_data,max_impressions,triggers,contextual_filters {clause_type,filters {filter_type" +
            ",unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_valu" +
            "e,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,st" +
            "ring_value},extra_datas {name,required,bool_value,int_value,string_value}}}}}},is_uncancelable,template {name,parameters {name,required,bool_value,string_value,color_value,}},creatives {title {text},content {text},footer {text},social_context {text},social_context_images,primary_action{title {" +
            "text},url,limit,dismiss_promotion},secondary_action{title {text},url,limit,dismiss_promotion},dismiss_action{title {text},url,limit,dismiss_promotion},image.scale(<scale>) {uri,width,height}}}}}}}\",\"5734\":\"Query QuickPromotionSurfaceQuery: Viewer {viewer() {eligible_promotions.trigger_cont" +
            "ext_v2(<trigger_context_v2>).ig_parameters(<ig_parameters>).trigger_name(<trigger_name>).surface_nux_id(<surface>).external_gating_permitted_qps(<external_gating_permitted_qps>).supports_client_filters(true).include_holdouts(true) {edges {client_ttl_seconds,log_eligibility_waterfall,is_holdout" +
            ",priority,time_range {start,end},node {id,promotion_id,logging_data,max_impressions,triggers,contextual_filters {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {claus" +
            "e_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,r" +
            "equired,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}}}}}},is_uncancelable,template {name,parameters {name,required,bool_value,st" +
            "ring_value,color_value,}},creatives {title {text},content {text},footer {text},social_context {text},social_context_images,primary_action{title {text},url,limit,dismiss_promotion},secondary_action{title {text},url,limit,dismiss_promotion},dismiss_action{title {text},url,limit,dismiss_promotion" +
            "},image.scale(<scale>) {uri,width,height}}}}}}}\"}";

        public const string SURFACES_TO_TRIGGERS_HASHTAG_FEED_TOOLTIP = "{\"5858\":[\"instagram_hashtag_feed_tooltip\"]}";
        public const string SURFACES_TO_QUERIES_HASHTAG_FEED_TOOLTIP = "{\"5858\":\"Query QuickPromotionSurfaceQuery: Viewer {viewer() {eligible_promotions.trigger_context_v2(<trigger_context_v2>).ig_parameters(<ig_parameters>).trigger_name(<trigger_name>).surface_nux_id(<surface>).external_gating_permitte" +
            "d_qps(<external_gating_permitted_qps>).supports_client_filters(true).include_holdouts(true) {edges {client_ttl_seconds,log_eligibility_waterfall,is_holdout,priority,time_range {start,end},node {id,promotion_id,logging_data,max_impressions,triggers,contextual_filters {clause_type,filters {filte" +
            "r_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,in" +
            "t_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_va" +
            "lue,string_value},extra_datas {name,required,bool_value,int_value,string_value}}}}}},is_uncancelable,template {name,parameters {name,required,bool_value,string_value,color_value,}},creatives {title {text},content {text},footer {text},social_context {text},social_context_images,primary_action{t" +
            "itle {text},url,limit,dismiss_promotion},secondary_action{title {text},url,limit,dismiss_promotion},dismiss_action{title {text},url,limit,dismiss_promotion},image.scale(<scale>) {uri,width,height}}}}}}}\"}";
        
        public const string SURFACES_TO_TRIGGERS_GET_USER_PROFILE = "{\"4715\":[],\"5858\":[],\"5734\":[]}";
        public const string SURFACES_TO_QUERIES_GET_USER_PROFILE = "{\"4715\":\"Query QuickPromotionSurfaceQuery: Viewer {viewer() {eligible_promotions.trigger_context_v2(<trigger_context_v2>).ig_parameters(<ig_parameters>).trigger_name(<trigger_name>).surface_nux_id(<surface>).external_gating_permitted_qp" +
            "s(<external_gating_permitted_qps>).supports_client_filters(true).include_holdouts(true) {edges {client_ttl_seconds,log_eligibility_waterfall,is_holdout,priority,time_range {start,end},node {id,promotion_id,logging_data,max_impressions,triggers,contextual_filters {clause_type,filters {filter_ty" +
            "pe,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_va" +
            "lue,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value," +
            "string_value},extra_datas {name,required,bool_value,int_value,string_value}}}}}},is_uncancelable,template {name,parameters {name,required,bool_value,string_value,color_value,}},creatives {title {text},content {text},footer {text},social_context {text},social_context_images,primary_action{title" +
            " {text},url,limit,dismiss_promotion},secondary_action{title {text},url,limit,dismiss_promotion},dismiss_action{title {text},url,limit,dismiss_promotion},image.scale(<scale>) {uri,width,height}}}}}}}\",\"5858\":\"Query QuickPromotionSurfaceQuery: Viewer {viewer() {eligible_promotions.trigger_co" +
            "ntext_v2(<trigger_context_v2>).ig_parameters(<ig_parameters>).trigger_name(<trigger_name>).surface_nux_id(<surface>).external_gating_permitted_qps(<external_gating_permitted_qps>).supports_client_filters(true).include_holdouts(true) {edges {client_ttl_seconds,log_eligibility_waterfall,is_holdo" +
            "ut,priority,time_range {start,end},node {id,promotion_id,logging_data,max_impressions,triggers,contextual_filters {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {cla" +
            "use_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name" +
            ",required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}}}}}},is_uncancelable,template {name,parameters {name,required,bool_value," +
            "string_value,color_value,}},creatives {title {text},content {text},footer {text},social_context {text},social_context_images,primary_action{title {text},url,limit,dismiss_promotion},secondary_action{title {text},url,limit,dismiss_promotion},dismiss_action{title {text},url,limit,dismiss_promoti" +
            "on},image.scale(<scale>) {uri,width,height}}}}}}}\",\"5734\":\"Query QuickPromotionSurfaceQuery: Viewer {viewer() {eligible_promotions.trigger_context_v2(<trigger_context_v2>).ig_parameters(<ig_parameters>).trigger_name(<trigger_name>).surface_nux_id(<surface>).external_gating_permitted_qps(<e" +
            "xternal_gating_permitted_qps>).supports_client_filters(true).include_holdouts(true) {edges {client_ttl_seconds,log_eligibility_waterfall,is_holdout,priority,time_range {start,end},node {id,promotion_id,logging_data,max_impressions,triggers,contextual_filters {clause_type,filters {filter_type,u" +
            "nknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value," +
            "string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,stri" +
            "ng_value},extra_datas {name,required,bool_value,int_value,string_value}}}}}},is_uncancelable,template {name,parameters {name,required,bool_value,string_value,color_value,}},creatives {title {text},content {text},footer {text},social_context {text},social_context_images,primary_action{title {te" +
            "xt},url,limit,dismiss_promotion},secondary_action{title {text},url,limit,dismiss_promotion},dismiss_action{title {text},url,limit,dismiss_promotion},image.scale(<scale>) {uri,width,height}}}}}}}\"}";
        
        public const string SURFACES_TO_TRIGGERS_INSTAGRAM_SURVEY = "{\"5734\":[\"instagram_survey\"]}";
        public const string SURFACES_TO_QUERIES_INSTAGRAM_SURVEY = "{\"5734\":\"Query QuickPromotionSurfaceQuery: Viewer {viewer() {eligible_promotions.trigger_context_v2(<trigger_context_v2>).ig_parameters(<ig_parameters>).trigger_name(<trigger_name>).surface_nux_id(<surface>).external_gating_permitted_q" +
            "ps(<external_gating_permitted_qps>).supports_client_filters(true).include_holdouts(true) {edges {client_ttl_seconds,log_eligibility_waterfall,is_holdout,priority,time_range {start,end},node {id,promotion_id,logging_data,max_impressions,triggers,contextual_filters {clause_type,filters {filter_" +
            "type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int" +
            "_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_value,string_value},extra_datas {name,required,bool_value,int_value,string_value}},clauses {clause_type,filters {filter_type,unknown_action,value {name,required,bool_value,int_va" +
            "lue,string_value},extra_datas {name,required,bool_value,int_value,string_value}}}}}},is_uncancelable,template {name,parameters {name,required,bool_value,string_value,color_value,}},creatives {title {text},content {text},footer {text},social_context {text},social_context_images,primary_action{" +
            "title {text},url,limit,dismiss_promotion},secondary_action{title {text},url,limit,dismiss_promotion},dismiss_action{title {text},url,limit,dismiss_promotion},image.scale(<scale>) {uri,width,height}}}}}}}\"}";

        #endregion

        public const string DIRECT_THREAD_VIDEOCALLS_MUTE = API_SUFFIX + "/direct_v2/threads/{0}/mute_video_call/";
        public const string DIRECT_THREAD_VIDEOCALLS_UNMUTE = API_SUFFIX + "/direct_v2/threads/{0}/unmute_video_call/";
        public const string DIRECT_THREAD_REMOVE_ALL_USERS = API_SUFFIX + "/direct_v2/threads/{0}/remove_all_users/";
        public const string DIRECT_THREAD_APPROVAL_REQUIRED_FOR_NEW_MEMBERS = API_SUFFIX + "/direct_v2/threads/{0}/approval_required_for_new_members/";
        public const string DIRECT_THREAD_APPROVAL_NOT_REQUIRED_FOR_NEW_MEMBERS = API_SUFFIX + "/direct_v2/threads/{0}/approval_not_required_for_new_members/";
        public const string DIRECT_THREAD_ADD_ADMINS = API_SUFFIX + "/direct_v2/threads/{0}/add_admins/";
        public const string DIRECT_THREAD_REMOVE_ADMINS = API_SUFFIX + "/direct_v2/threads/{0}/remove_admins/";
        public const string DIRECT_THREAD_REMOVE_USERS = API_SUFFIX + "/direct_v2/threads/{0}/remove_users/";
        public const string DIRECT_THREAD_BY_PARTICIPANTS = API_SUFFIX + "/direct_v2/threads/get_by_participants/?recipient_users=[{0}]&limit={1}";

        public const string DISCOVER_DISMISS_USER_SUGGESTION = API_SUFFIX + "/discover/aysf_dismiss/";
        public const string EXPLORE_CHANNEL_VIEWER = API_SUFFIX + "/channels/viewer/discover_videos/{0}/";

        public const string RUPLOAD_IGVIDEO_START = "/rupload_igvideo/{0}?segmented=true&phase=start";
        public const string RUPLOAD_IGVIDEO_TRANSFER = "/rupload_igvideo/{0}-{1}-{2}?segmented=true&phase=transfer";
        public const string RUPLOAD_IGVIDEO_END = "/rupload_igvideo/{0}?segmented=true&phase=end";

        public const int FEED_VIEW_INFO_VERSION = 24;
        public const int MAX_TIMELINE_STARING_MILLISECONDS = 6000;

        public const int MIN_BANDWIDTH_SPEED_KBPS = 400;
        public const int MAX_BANDWIDTH_SPEED_KBPS = 2000;
        public const int MIN_BANDWIDTH_SPEED_BPS = 100;
        public const int MAX_BANDWIDTH_SPEED_BPS = 999;

        public const int MIN_BANDWIDTH_TOTAL_TIME_MS = 100;
        public const int MAX_BANDWIDTH_TOTAL_TIME_MS = 3000;

        // push 
        public const string FACEBOOK_OTA_FIELDS = "update%7Bdownload_uri%2Cdownload_uri_delta_base%2Cversion_code_delta_base%2Cdownload_uri_delta%2Cfallback_to_full_update%2Cfile_size_delta%2Cversion_code%2Cpublished_date%2Cfile_size%2Cota_bundle_type%2Cresources_checksum%7D";
        public const int FACEBOOK_ORCA_PROTOCOL_VERSION = 20150314;
        public const string FACEBOOK_ORCA_APP_ID = "540921560862818";
        public const string FACEBOOK_ANALYTICS_APP_ID = "567310203415052";
        public const string INSTAGRAM_PACKAGE_NAME = "com.instagram.android";

        #endregion New


        #region Main

        public const string HEADER_PIGEON_SESSION_ID = "X-Pigeon-Session-Id";
        public const string HEADER_PIGEON_RAWCLINETTIME = "X-Pigeon-Rawclienttime";
        public const string HEADER_X_IG_CONNECTION_SPEED = "X-IG-Connection-Speed";
        public const string HEADER_X_IG_BANDWIDTH_SPEED_KBPS = "X-IG-Bandwidth-Speed-KBPS";
        public const string HEADER_X_IG_BANDWIDTH_TOTALBYTES_B = "X-IG-Bandwidth-TotalBytes-B";
        public const string HEADER_X_IG_BANDWIDTH_TOTALTIME_MS = "X-IG-Bandwidth-TotalTime-MS";

        public const string HEADER_RESPONSE_AUTHORIZATION = "ig-set-authorization";
        public const string HEADER_X_WWW_CLAIM = "X-IG-WWW-Claim";
        public const string HEADER_X_WWW_CLAIM_DEFAULT = "0";
        public const string HEADER_RESPONSE_X_WWW_CLAIM = "x-ig-set-www-claim";
        public const string HEADER_X_FB_TRIP_ID = "X-FB-TRIP-ID";
        public const string HEADER_IG_APP_STARTUP_COUNTRY = "X-IG-App-Startup-Country";
        public static string HEADER_IG_APP_STARTUP_COUNTRY_VALUE = "PL"; // IR
        public const string HEADER_X_FB_HTTP_ENGINE = "X-FB-HTTP-Engine";
        public const string HEADER_X_IG_APP_LOCALE = "X-IG-App-Locale";
        public const string HEADER_X_IG_MAPPED_LOCALE = "X-IG-Mapped-Locale";
        public const string HEADER_X_IG_DEVICE_LOCALE = "X-IG-Device-Locale";
        public const string HEADER_X_MID = "X-MID";
        public const string HEADER_AUTHORIZATION = "Authorization";
        public const string COOKIES_MID = "mid";
        public const string COOKIES_DS_USER_ID = "ds_user_id";
        public const string COOKIES_SESSION_ID = "sessionid";

        public const string HEADER_X_IG_EXTENDED_CDN_THUMBNAIL_CACHE_BUSTING_VALUE = "X-IG-Extended-CDN-Thumbnail-Cache-Busting-Value";
        public const string HEADER_X_IG_BLOKS_VERSION_ID = "X-Bloks-Version-Id";
        public const string HEADER_X_IG_BLOKS_IS_LAYOUT_RTL = "X-Bloks-Is-Layout-RTL";
        public const string HEADER_X_IG_BLOKS_ENABLE_RENDERCODE = "X-Bloks-Enable-RenderCore";
        public const string HEADER_X_IG_DEVICE_ID = "X-IG-Device-ID";
        public const string HEADER_X_IG_ANDROID_ID = "X-IG-Android-ID";
        public const string CURRENT_BLOKS_VERSION_ID = "0e9b6d9c0fb2a2df4862cd7f46e3f719c55e9f90c20db0e5d95791b66f43b367";

        public const string ACCEPT_ENCODING = "gzip, deflate"; //sdch?
        public const string API = "/api";
        public const string API_SUFFIX = API + API_VERSION;
        public const string API_SUFFIX_V2 = API + API_VERSION_V2;
        public const string API_VERSION = "/v1";
        public const string API_VERSION_V2 = "/v2";
        public const string BASE_INSTAGRAM_API_URL = INSTAGRAM_URL + API_SUFFIX + "/";
        public const string COMMENT_BREADCRUMB_KEY = "iN4$aGr0m";
        public const string CSRFTOKEN = "csrftoken";
        public const string HEADER_ACCEPT_ENCODING = "Accept-Encoding";
        public const string HEADER_ACCEPT_LANGUAGE = "Accept-Language";
        public const string HEADER_COUNT = "count";
        public const string HEADER_EXCLUDE_LIST = "exclude_list";
        public const string HEADER_IG_APP_ID = "X-IG-App-ID";
        public const string HEADER_IG_CAPABILITIES = "X-IG-Capabilities";
        public const string HEADER_IG_CONNECTION_TYPE = "X-IG-Connection-Type";
        public const string HEADER_IG_SIGNATURE = "signed_body";
        public const string HEADER_IG_SIGNATURE_KEY_VERSION = "ig_sig_key_version";
        public const string HEADER_MAX_ID = "max_id";
        public const string HEADER_PHONE_ID = "phone_id";
        public const string HEADER_QUERY = "q";
        public const string HEADER_RANK_TOKEN = "rank_token";
        public const string HEADER_TIMEZONE = "timezone_offset";
        public const string HEADER_USER_AGENT = "User-Agent";
        public const string HEADER_X_INSTAGRAM_AJAX = "X-Instagram-AJAX";
        public const string HEADER_X_REQUESTED_WITH = "X-Requested-With";
        public const string HEADER_XCSRF_TOKEN = "X-CSRFToken";
        public const string HEADER_XGOOGLE_AD_IDE = "X-Google-AD-ID";
        public const string HEADER_XML_HTTP_REQUEST = "XMLHttpRequest";
        public const string IG_APP_ID = "567067343352427";
        public const string IG_CONNECTION_TYPE = "WIFI";
        public const string IG_SIGNATURE_KEY_VERSION = "4";
        public const string INSTAGRAM_URL = "https://i.instagram.com";
        public const string P_SUFFIX = "p/";
        public const string SUPPORTED_CAPABALITIES_HEADER = "supported_capabilities_new";

        public static string TIMEZONE = "Europe/Warsaw";

        public static int TIMEZONE_OFFSET = 3600;

        public const string USER_AGENT =
                                    "Instagram {6} Android ({7}/{8}; {0}; {1}; {2}/{10}; {3}; {4}; {5}; en_US; {9})";
        public const string USER_AGENT_DEFAULT =
        "Instagram 135.0.0.28.119 Android (24/7.0; 480dpi; 1080x1920; Xiaomi/xiaomi; Redmi Note 4X; mido; qcom; en_US; 206670927)";
        
        public static readonly JArray SupportedCapabalities = new JArray
        {
            new JObject
            {
                {"name","SUPPORTED_SDK_VERSIONS"},
                {"value","45.0,46.0,47.0,48.0,49.0,50.0,51.0," +
                    "52.0,53.0,54.0,55.0,56.0,57.0,58.0,59.0," +
                    "60.0,61.0,62.0,63.0,64.0,65.0,66.0,67.0," +
                    "68.0,69.0,70.0,71.0,72.0,73.0,74.0,75.0," +
                    "76.0,77.0,78.0,79.0,80.0,81.0,82.0,83.0," +
                    "84.0,85.0"}
            },
            new JObject
            {
                {"name","FACE_TRACKER_VERSION"},
                {"value","14"}
            },
            new JObject
            {
                {"name","COMPRESSION"},
                {"value","ETC2_COMPRESSION"}
            },
            new JObject
            {
                {"name","world_tracker"},
                {"value","world_tracker_enabled"}
            },
            new JObject
            {
                {"name","gyroscope"},
                {"value","gyroscope_enabled"}
            }
        };

        #region Experiment Constants

        public const string LOGIN_EXPERIMENTS_CONFIGS = "ig_growth_android_profile_pic_prefill_with_fb_pic_2,ig_android_email_fuzzy_matching_universe,i" +
            "g_android_shorten_sac_for_one_eligible_main_account_universe,ig_android_fix_sms_read_lollipop,ig_android_mas_remove_close_friends_entrypoi" +
            "nt,ig_android_device_verification_fb_signup,ig_android_reg_nux_headers_cleanup_universe,ig_android_direct_main_tab_universe_v2,ig_android_" +
            "modularized_dynamic_nux_universe,ig_android_account_linking_upsell_universe,ig_android_spatial_account_switch_universe,ig_android_enable_k" +
            "eyboardlistener_redesign,ig_android_suma_landing_page,ig_android_notification_unpack_universe,ig_android_access_flow_prefill,ig_android_se" +
            "condary_account_creation_universe,ig_android_shortcuts_2019,ig_android_ask_for_permissions_on_reg,ig_android_device_based_country_verifica" +
            "tion,ig_account_identity_logged_out_signals_global_holdout_universe,ig_android_video_ffmpegutil_pts_fix,ig_android_login_identifier_fuzzy_" +
            "match,ig_android_black_out_toggle_universe,ig_android_security_intent_switchoff,ig_android_one_login_toast_universe,ig_android_mas_notific" +
            "ation_badging_universe,ig_android_log_suggested_users_cache_on_error,ig_android_multi_tap_login_new,ig_android_nux_add_email_device,ig_act" +
            "ivation_global_discretionary_sms_holdout,ig_android_fci_onboarding_friend_search,ig_android_fb_account_linking_sampling_freq_universe,ig_a" +
            "ndroid_device_info_foreground_reporting,ig_assisted_login_universe,ig_android_video_render_codec_low_memory_gc,ig_android_direct_add_direc" +
            "t_to_android_native_photo_share_sheet,ig_android_device_detection_info_upload,ig_android_hsite_prefill_new_carrier,ig_android_one_tap_aymh" +
            "_redesign_universe,ig_android_recovery_one_tap_holdout_universe,ig_android_gmail_oauth_in_reg,ig_android_reg_modularization_universe,ig_an" +
            "droid_passwordless_auth,ig_android_sim_info_upload,ig_android_universe_noticiation_channels,ig_android_sign_in_help_only_one_account_famil" +
            "y_universe,ig_android_hide_contacts_list_in_nux,ig_android_pwd_encrytpion,ig_android_sac_follow_from_other_accounts_nux_universe,ig_androi" +
            "d_account_recovery_auto_login,ig_android_targeted_one_tap_upsell_universe,ig_video_debug_overlay,ig_android_caption_typeahead_fix_on_o_uni" +
            "verse,ig_android_direct_remove_view_mode_stickiness_universe,ig_android_prefetch_debug_dialog,ig_android_retry_create_account_universe,ig_" +
            "android_quickcapture_keep_screen_on,ig_android_smartlock_hints_universe,ig_android_passwordless_account_password_creation_universe,ig_andr" +
            "oid_onetaplogin_optimization,ig_android_family_apps_user_values_provider_universe,ig_android_registration_confirmation_code_universe,ig_an" +
            "droid_mobile_http_flow_device_universe,ig_android_direct_send_like_from_notification,ig_android_get_cookie_with_concurrent_session_univers" +
            "e,ig_android_push_fcm,ig_android_device_info_job_based_reporting,ig_android_new_users_one_tap_holdout_universe,ig_android_vc_interop_use_t" +
            "est_igid_universe,ig_android_device_verification_separate_endpoint,ig_android_sms_retriever_backtest_universe";

        public const string AFTER_LOGIN_EXPERIMENTS_CONFIGS = "ig_view_preallocation_universe,ig_cameracore_android_camera2_focus_after_first_frame,ig_" +
            "android_explore_reel_loading_state,ig_android_whitehat_options_universe,ig_android_test_not_signing_address_book_unlink_endpoint,ig_androi" +
            "d_fb_profile_integration_fbnc_universe,ig_android_delete_ssim_compare_img_soon,ig_android_direct_reel_fetching,ig_android_intialization_ch" +
            "unk_410,ig_android_le_videoautoplay_disabled,ig_android_render_thread_memory_leak_holdout,ig_android_camera_gallery_upload_we_universe,ig_" +
            "android_shopping_checkout_signaling,ig_android_realtime_mqtt_logging,ig_android_video_exoplayer_2,ig_android_use_action_sheet_for_media_ov" +
            "erflow,ig_location_tagging_product_universe,ig_android_ad_async_ads_universe,ig_explore_reel_ring_universe,android_camera_core_cpu_frames_" +
            "sync,ig_smb_ads_holdout_2019_h2_universe,coupon_price_test_ad4ad_instagram_resurrection_universe,ig_camera_android_effect_metadata_cache_r" +
            "efresh_universe,ig_android_payments_growth_promote_payments_without_payments,ig_android_dash_script,ig_interactions_h1_2019_team_holdout_u" +
            "niverse,ig_android_image_exif_metadata_ar_effect_id_universe,ig_android_business_attribute_sync,ig_android_follow_request_button_improveme" +
            "nts_universe,ig_android_appstate_logger,ig_payment_checkout_info,ig_android_search_remove_null_state_sections,ig_stories_allow_camera_acti" +
            "ons_while_recording,ig_android_delay_on_sticker_search_universe,ig_android_unified_iab_logging_universe,ig_android_disable_manual_retries," +
            "ig_android_optic_new_architecture,ig_android_ig_to_fb_sync_universe,ig_android_search_usl,ig_android_profile_thumbnail_impression,ig_andro" +
            "id_fbc_upsell_on_dp_first_load,ig_branded_content_tagging_approval_request_flow_brand_side_v2,ig_android_rename_share_option_in_dialog_men" +
            "u_universe,ig_android_multi_author_story_reshare_universe,android_cameracore_fbaudio_integration_ig_universe,ig_shopping_checkout_ig_funde" +
            "d_incentives,ig_android_wellbeing_support_frx_igtv_reporting,ig_android_igtv_autoplay_on_prepare,coupon_price_test_boost_instagram_media_a" +
            "cquisition_universe,ig_android_not_interested_secondary_options,ig_android_business_transaction_in_stories_creator,ig_android_feed_short_s" +
            "ession_new_post_pill,ig_android_vc_explicit_intent_for_notification,ig_android_audience_control,ig_shopping_insights_wc_copy_update_androi" +
            "d,ig_android_unfollow_reciprocal_universe,ig_promote_enter_error_screens_universe,ig_android_delayed_comments,ig_android_recognition_track" +
            "ing_thread_prority_universe,ig_android_business_transaction_in_stories_consumer,ig_android_vc_cpu_overuse_universe,ig_android_purx_native_" +
            "checkout_universe,ig_profile_company_holdout_h2_2018,ig_android_analytics_background_uploader_schedule,ig_android_user_url_deeplink_fbpage" +
            "_endpoint,ig_android_insights_activity_tab_native_universe,ig_android_iris_improvements,ig_android_show_self_followers_after_becoming_priv" +
            "ate_universe,ig_android_image_pdq_calculation,ig_android_ad_watchbrowse_universe,ig_android_photo_creation_large_width,ig_android_interest" +
            "_follows_universe,ig_android_ig_personal_account_xpost_eligibility_from_server,ig_android_dual_destination_quality_improvement,ig_company_" +
            "profile_holdout,ig_android_invite_list_button_redesign_universe,ig_android_vc_codec_settings,ig_android_raven_video_segmented_upload_raven" +
            "_only_universe,ig_rti_inapp_notifications_universe,ig_android_fix_reshare_xposting_killswitch_universe,ig_android_branded_content_insights" +
            "_disclosure,ig_android_fb_follow_server_linkage_universe,ig_ads_experience_holdout_2019_h2,ard_ig_broti_effect,ig_android_payments_growth_" +
            "promote_payments_in_payments,ig_android_enable_zero_rating,ig_android_audience,ig_android_context_feed_recycler_view,ig_android_mainfeed_g" +
            "enerate_prefetch_background,ig_hashtag_display_universe,ig_android_ig_personal_account_to_fb_page_linkage_backfill,ig_android_live_qa_view" +
            "er_v1_universe,ig_android_branded_content_upsell_keywords_extension,ig_interactions_h2_2019_team_holdout_universe,ig_android_stories_music" +
            "_lyrics,ig_android_music_story_fb_crosspost_universe,ig_android_mqtt_cookie_auth_memcache_universe,ig_camera_android_feed_effect_attributi" +
            "on_universe,ig_android_bullying_warning_system_2019h2,ig_android_jp_map_location_sticker_universe,ig_android_q3lc_transparency_control_set" +
            "tings,ig_android_wellbeing_support_frx_friction_process_education,ig_threads_sanity_check_thread_viewkeys,ig_android_push_notifications_se" +
            "ttings_redesign_universe,ig_explore_2019_h1_video_autoplay_resume,ig_android_sidecar_segmented_streaming_universe,ig_android_profile_unifi" +
            "ed_creation_universe,ig_android_use_action_sheet_for_profile_overflow,ig_android_persistent_nux,instagram_pcp_qp_redesign_universe,ig_fb_n" +
            "otification_universe,ig_email_sent_list_universe,ig_android_feed_camera_size_setter,ig_android_churned_find_friends_redirect_to_discover_p" +
            "eople,ig_mprotect_code_universe,ig_android_self_profile_suggest_business_main,instagram_stories_time_fixes,ig_direct_android_mentions_all_" +
            "universe,ig_android_vc_capture_universe,ig_android_camera_recents_gallery_modal,ig_android_follow_requests_ui_improvements,ig_android_qr_c" +
            "ode_nametag,ig_search_client_cache_overriding_universe,ig_android_ad_view_ads_native_universe,ig_android_account_insights_shopping_content" +
            "_universe,ig_android_temp_file_cleanup,ig_new_eof_demarcator_universe,ig_android_show_muted_accounts_page,ig_android_suggested_users_backg" +
            "round,ig_android_create_mode_memories_see_all,ig_android_network_perf_qpl_ppr,ig_android_promote_migration_gamma_universe,ig_android_migra" +
            "te_gifs_to_ig_endpoint,ig_android_on_notification_cleared_async_universe,ig_android_optic_use_new_bitmap_photo_api,ig_android_ads_manager_" +
            "pause_resume_ads_universe,ig_sim_api_analytics_reporting,ig_promote_ctd_post_insights_universe,ig_android_browser_ads_page_content_width_u" +
            "niverse,ig_android_direct_unread_count_badge,ig_android_wellbeing_support_frx_cowatch_reporting,ig_android_viewpoint_occlusion,ig_android_" +
            "sharedpreferences_qpl_logging,ig_promote_django_error_handling,ig_android_vc_cowatch_universe,ig_android_self_story_button_non_fbc_account" +
            "s,ig_android_inline_editing_local_prefill,ig_close_friends_v4,ig_aggregated_quick_reactions,ig_android_stories_music_lyrics_pre_capture,in" +
            "stagram_ns_qp_prefetch_universe,ig_android_optic_photo_cropping_fixes,ig_android_igtv_first_frame_cover,ig_android_direct_bump_active_thre" +
            "ads,ig_android_stories_send_client_reels_on_tray_fetch_universe,ig_android_live_realtime_comments_universe,ig_early_friending_holdout_univ" +
            "erse,ig_graph_management_h2_2019_universe,ig_android_tagging_activity_video_preview,ig_android_push_reliability_universe,ig_discovery_2019" +
            "_h2_holdout_universe,ig_badge_dedup_universe,ig_android_multi_capture_camera,ig_inventory_connections,ig_smb_ads_holdout_2018_h2_universe," +
            "ig_android_stories_cross_sharing_to_fb_holdout_universe,ig_android_edit_location_page_info,ig_android_fb_profile_integration_universe,ig_b" +
            "iz_growth_insights_universe,ig_android_shopping_pdp_post_purchase_sharing,ig_android_direct_multi_upload_universe,ig_android_stories_webli" +
            "nk_creation,ig_android_multi_dex_class_loader_v2,ig_android_product_breakdown_post_insights,ig_android_felix_video_upload_length,ig_androi" +
            "d_pending_media_file_registry,ig_android_ad_stories_scroll_perf_universe,ig_android_biz_ranked_requests_universe,ig_android_igsystrace_uni" +
            "verse,android_cameracore_safe_makecurrent_ig,aymt_instagram_promote_flow_abandonment_ig_universe,ig_android_direct_mutation_manager_media_" +
            "3,ig_android_camera_formats_ranking_universe,ig_android_story_bottom_sheet_clips_single_audio_mas,ig_android_story_ads_performance_univers" +
            "e_1,ig_android_video_live_trace_universe,ig_commerce_platform_ptx_bloks_universe,ig_close_friends_v4_global,ig_direct_holdout_h2_2018,ig_a" +
            "ndroid_mentions_suggestions,ig_android_memory_use_logging_universe,ig_android_stories_samsung_sharing_integration,ig_android_igtv_player_f" +
            "ollow_button,ig_android_shimmering_loading_state,ig_android_direct_pending_media,ig_adapter_leak_universe,ig_camera_android_facetracker_v1" +
            "2_universe,ig_android_follow_button_in_story_viewers_list,ig_android_automated_logging,ig_android_viewpoint_stories_public_testing,ig_prom" +
            "ote_net_promoter_score_universe,ig_promote_default_destination_universe,ig_android_branded_content_access_tag,ig_android_reel_raven_video_" +
            "segmented_upload_universe,ig_shop_directory_entrypoint,text_mode_text_overlay_refactor_universe,ig_memory_manager_universe,ig_android_sso_" +
            "use_trustedapp_universe,ig_android_igtv_home_icon,ig_android_remove_follow_all_fb_list,ig_android_recipient_picker,ig_android_video_stream" +
            "ing_upload_universe,ig_android_direct_inbox_vm_actions,ig_android_save_all,ig_android_direct_block_from_group_message_requests,ig_camera_a" +
            "ndroid_subtle_filter_universe,ig_android_live_qa_broadcaster_v1_universe,ig_android_product_tag_suggestions,ig_direct_holdout_h1_2019,ig_a" +
            "ndroid_vc_join_timeout_universe,ig_prefetch_scheduler_backtest,ig_android_shopping_bag_null_state_v1,ig_android_refresh_empty_feed_su_univ" +
            "erse,ig_android_hashtag_remove_share_hashtag,ig_android_media_streaming_sdk_universe,ig_android_spark_arengine_igl_activations,ig_android_" +
            "feed_interactions_carousel_overscoll,ig_shopping_bag_universe,ig_biz_graph_connection_universe,instagram_pcp_activity_feed_following_tab_u" +
            "niverse,ig_android_dismiss_recent_searches,ig_challenge_general_v2,ig_android_creator_quick_reply_universe,ig_android_direct_use_refactore" +
            "d_sender_avatar,ig_android_save_to_collections_bottom_sheet_refactor,ig_camera_effect_frx_categories_universe,ig_android_startupmanager_re" +
            "factor,ig_android_dropframe_manager,ig_android_wellbeing_support_frx_direct_reporting,ig_android_business_remove_unowned_fb_pages,ig_graph" +
            "_chain_unfollow_universe,ig_android_ads_bottom_sheet_report_flow,ig_android_qpl_class_marker,qe_android_direct_vm_view_modes,ig_android_un" +
            "follow_from_main_feed_v2,ig_android_livewith_liveswap_optimization_universe,ig_android_disk_usage_logging_universe,ig_android_live_renderi" +
            "ng_looper_universe,ig_android_bandwidth_timed_estimator,ig_android_video_dup_request_timeout,ig_android_cover_frame_upload_skip_story_rave" +
            "n_universe,ig_shopping_merchant_profile_bag_android,ig_android_business_cross_post_with_biz_id_infra,ig_android_jp_saved_collection_map_un" +
            "iverse,ig_android_stories_gallery_video_segmentation,ig_android_live_egl10_compat,ig_android_vc_face_effects_universe,ig_android_keyword_m" +
            "edia_serp_page,ig_android_story_camera_share_to_feed_universe,ig_android_jp_multi_media_edit_redesign_universe,ig_android_camera_async_set" +
            "up_lowend_focus,ig_ar_shopping_camera_android_universe,ig_android_skip_button_content_on_connect_fb_universe,ig_android_stories_blacklist," +
            "ig_android_tango_cpu_overuse_universe,ig_android_ad_watchbrowse_carousel_universe,ig_explore_2018_post_chaining_account_recs_dedupe_univer" +
            "se,ig_android_igtv_upload_error_messages,ig_android_igtv_stories_preview,ig_iab_tti_holdout_universe,ig_android_self_following_v2_universe" +
            ",ig_android_album_picker_with_section_headers_universe,ig_android_wab_adjust_resize_universe,ig_android_stories_gutter_width_universe,ig_a" +
            "ndroid_crash_fix_detach_from_gl_context,ig_android_camera_focus_v2,ig_android_test_remove_button_main_cta_self_followers_universe,ig_andro" +
            "id_react_native_universe_kill_switch,ig_android_fs_new_gallery,ig_android_iab_clickid_universe,ig_android_mediauri_parse_optimization,ig_a" +
            "ndroid_fix_ppr_thumbnail_url,ig_direct_reshare_sharesheet_ranking,ig_android_qp_kill_switch,ig_android_recommend_accounts_destination_rout" +
            "ing_fix,ig_android_stories_music_line_by_line_cube_reveal_lyrics_sticker,ig_android_igtv_reshare,ig_android_quick_promote_universe,igdirec" +
            "t_android_animate_inbox_list_changes,ig_android_react_native_email_sms_settings_universe,ig_android_profile_follow_tab_hashtag_row_univers" +
            "e,ig_android_search_impression_logging_viewpoint,ig_android_shopping_pdp_product_videos,ig_android_vio_pipeline_universe,ig_android_buildi" +
            "ng_aymf_universe,ig_android_vc_shareable_moments_universe,ig_android_fb_url_universe,ig_android_vc_egl10_compat,ig_android_video_fit_scale" +
            "_type_igtv,ig_android_optic_face_detection,ig_android_direct_delete_or_block_from_message_requests,ig_android_direct_mqtt_send,ig_android_" +
            "wellbeing_support_frx_stories_reporting,ig_android_promote_native_migration_universe,ig_android_camera_3p_in_post,ig_android_iab_autofill," +
            "ig_android_enable_automated_instruction_text_ar,ar_engine_audio_fba_integration_instagram,ig_android_video_ssim_fix_compare_frame_index,ig" +
            "_graph_management_production_h2_2019_holdout_universe,ig_android_do_not_show_social_context_on_follow_list_universe,ig_android_login_oneta" +
            "p_upsell_universe,ig_video_experimental_encoding_consumption_universe,android_ard_ig_use_brotli_effect_universe,ig_android_wellbeing_suppo" +
            "rt_frx_live_reporting,ig_android_stories_share_extension_video_segmentation,ig_android_feed_cache_update,ig_android_image_upload_quality_u" +
            "niverse,ig_android_follow_requests_copy_improvements,igqe_pending_tagged_posts,ig_android_ads_history_universe,ig_android_direct_thread_ta" +
            "rget_queue_universe,ig_graph_evolution_holdout_universe,ig_camera_thread_colour_filter,ig_android_fix_push_setting_logging_universe,ig_and" +
            "roid_low_latency_consumption_universe,ig_android_graphql_survey_new_proxy_universe,ig_android_direct_inbox_recently_active_presence_dot_un" +
            "iverse,ig_android_igtv_explore2x2_viewer,ig_android_direct_message_follow_button,ig_android_wellbeing_support_frx_feed_posts_reporting,ig_" +
            "stories_ads_delivery_rules,ig_android_reel_tray_item_impression_logging_viewpoint,ig_android_direct_activator_cards,ig_android_frx_creatio" +
            "n_question_responses_reporting,ig_android_business_tokenless_stories_xposting,ig_android_canvas_cookie_universe,ig_android_stories_sundial" +
            "_creation_ar_effects,ig_end_of_feed_universe,ig_shopping_checkout_2x2_platformization_universe,ig_android_image_upload_skip_queue_only_on_" +
            "wifi,ig_android_ad_holdout_watchandmore_universe,ig_iab_use_default_intent_loading,ig_shopping_checkout_improvements_v2_universe,ig_promot" +
            "e_post_insights_entry_universe,ig_android_vc_cowatch_media_share_universe,ig_hero_player,ig_android_video_upload_transform_matrix_fix_univ" +
            "erse,ig_android_camera_reduce_file_exif_reads,ig_android_wellbeing_support_frx_profile_reporting,ig_android_xposting_feed_to_stories_resha" +
            "res_universe,ig_android_insights_media_hashtag_insight_universe,ig_android_igtv_whitelisted_for_web,ig_android_stories_music_sticker_posit" +
            "ion,ig_close_friends_list_suggestions,ig_android_video_raven_bitrate_ladder_universe,ig_smb_ads_holdout_2019_h1_universe,ig_default_teens_" +
            "to_private_universe,ig_android_vc_background_call_toast_universe,ig_android_stories_sundial_creation_universe,ig_camera_android_gallery_se" +
            "arch_universe,ig_android_downloadable_json_universe,ig_android_ard_ptl_universe,ig_android_fb_link_ui_polish_universe,ig_android_camera_gy" +
            "ro_universe,ig_android_location_integrity_universe,mi_viewpoint_viewability_universe,ig_android_business_promote_tooltip,ig_android_multi_" +
            "thread_sends,ig_android_shopping_bag_optimization_universe,ig_android_new_follower_removal_universe,ig_android_show_create_content_pages_u" +
            "niverse,ig_android_direct_leave_from_group_message_requests,ig_android_vc_cowatch_config_universe,ig_android_feed_post_warning_universe,ig" +
            "_android_branded_content_appeal_states,ig_android_do_not_show_social_context_for_likes_page_universe,ig_android_nametag,ig_android_zero_ra" +
            "ting_carrier_signal,ig_android_apr_lazy_build_request_infra,ig_android_video_ssim_fix_pts_universe,ig_ei_option_setting_universe,ig_androi" +
            "d_music_browser_redesign,ig_android_stories_quick_react_gif_universe,ig_shopping_pdp_more_related_product_section,ig_android_feed_core_ads" +
            "_2019_h1_holdout_universe,ig_android_igtv_crop_top,ig_android_claim_location_page,ig_android_direct_inbox_cache_universe,ig_android_place_" +
            "signature_universe,ig_android_frx_highlight_cover_reporting_qe,ig_android_explore_peek_redesign_universe,ig_android_emoji_util_universe_3," +
            "ig_android_account_insights_shopping_metrics_universe,ig_android_story_ads_2019_h1_holdout_universe,ig_android_video_visual_quality_score_" +
            "based_abr,ig_android_pbia_proxy_profile_universe,ig_android_unify_graph_management_actions,ig_android_ads_data_preferences_universe,ig_and" +
            "roid_ads_rendering_logging,ig_android_camera_upsell_dialog,ig_android_wishlist_reconsideration_universe,ig_android_vc_ringscreen_timeout_u" +
            "niverse,ig_android_expanded_xposting_upsell_directly_after_sharing_story_universe,ig_android_interactions_hide_keyboard_onscroll,ig_camera" +
            "_android_new_effect_gallery_entry_point_universe,ig_android_country_code_fix_universe,ig_cameracore_android_new_optic_camera2,ig_android_s" +
            "elf_story_setting_option_in_menu,ig_android_video_player_memory_leaks,ig_stories_ads_media_based_insertion,ig_android_logged_in_delta_migr" +
            "ation,ig_direct_android_bubble_system,ig_android_explore_discover_people_entry_point_universe,ig_android_stories_vpvd_container_module_fix" +
            ",ig_android_camera_class_preloading,ig_android_kitkat_segmented_upload_universe,ig_android_shopping_product_metadata_on_product_tiles_univ" +
            "erse,ig_android_direct_business_holdout,ig_camera_android_release_drawing_view_universe,ig_android_wellbeing_support_frx_comment_reporting" +
            ",ig_android_internal_sticker_universe,instagram_android_profile_follow_cta_context_feed,ig_android_story_sharing_holdout,ig_explore_2019_h" +
            "1_destination_cover,ig_android_hide_contacts_list,ig_android_fs_new_gallery_hashtag_prompts,ig_android_save_auto_sharing_to_fb_option_on_s" +
            "erver,ig_android_rainbow_hashtags,ig_android_vc_service_crash_fix_universe,instagram_pcp_bloks_qp_universe,ig_android_vc_missed_call_call_" +
            "back_action_universe,ig_emoji_render_counter_logging_universe,ig_android_stories_video_prefetch_kb,ig_android_live_subscribe_user_level_un" +
            "iverse,android_cameracore_preview_frame_listener2_ig_universe,ig_android_ad_iab_qpl_kill_switch_universe,ig_android_create_mode_tap_to_cyc" +
            "le,ig_android_igtv_feed_sharing_universe,igqe_cx_direct_two_tab_unread_indicator,ig_android_fb_sync_options_universe,ig_shopping_checkout_" +
            "improvements_universe,ig_android_ig_branding_in_fb_universe,ig_android_place_search_profile_image,ig_android_iab_holdout_universe,ig_andro" +
            "id_stories_disable_highlights_media_preloading,ig_payments_billing_address,ig_android_logging_metric_universe_v2,ig_fb_graph_differentiati" +
            "on,ig_feed_video_autoplay_stop_threshold,ig_stories_rainbow_ring,ig_android_direct_add_member_dialog_universe,ig_android_direct_default_gr" +
            "oup_name,ig_android_separate_empty_feed_su_universe,ig_android_product_tag_hint_dots,ig_promote_hide_local_awareness_universe,ig_business_" +
            "new_value_prop_universe,ig_android_wellbeing_support_frx_hashtags_reporting,ig_android_igtv_browse_button,ig_xposting_biz_feed_to_story_re" +
            "share,ig_android_whats_app_contact_invite_universe,ig_android_video_abr_universe,ig_android_feed_auto_share_to_facebook_dialog,ig_share_to" +
            "_story_toggle_include_shopping_product,ig_android_list_adapter_prefetch_infra,shop_home_hscroll_see_all_button_universe,ig_android_sidecar" +
            "_report_ssim,ig_feed_content_universe,ig_android_keyword_serp_recyclerview,ig_android_video_call_finish_universe,ig_hashtag_following_hold" +
            "out_universe,ig_android_fbpage_on_profile_side_tray,ig_android_xposting_dual_destination_shortcut_fix,ig_android_downloadable_fonts_univer" +
            "se,ig_android_stories_music_search_typeahead,direct_unread_reminder_qe,ig_android_stories_boomerang_v2_universe,ig_android_search_condense" +
            "d_search_icons,ig_promote_are_you_sure_universe,ig_android_camera_hair_segmentation_universe,ig_shopping_checkout_mvp_experiment,ig_androi" +
            "d_gif_preview_quality_universe,ig_camera_android_paris_filter_universe,ig_android_ttcp_improvements,ig_timestamp_public_test,android_ard_i" +
            "g_cache_size,ig_android_share_publish_page_universe,ig_android_stories_project_eclipse,ig_cameracore_android_new_optic_camera2_galaxy,ig_c" +
            "amera_android_device_capabilities_experiment,ig_android_analytics_mark_events_as_offscreen,ig_hashtag_creation_universe,ig_android_stories" +
            "_context_sheets_universe,ig_android_search_nearby_places_universe,ig_android_create_mode_templates,ig_android_ufiv3_holdout,ig_android_vis" +
            "ualcomposer_inapp_notification_universe,ig_android_xposting_upsell_directly_after_sharing_to_story,ig_android_integrity_sprint_universe,ig" +
            "_android_camera_tti_improvements,ig_promote_interactive_poll_sticker_igid_universe,ig_android_interactions_fix_activity_feed_ufi,ig_quick_" +
            "story_placement_validation_universe,ig_android_aggressive_media_cleanup,ig_android_insights_native_post_universe,ig_android_direct_media_l" +
            "atency_optimizations,ig_android_direct_remix_visual_messages,ig_android_video_outputsurface_handlerthread_universe,ig_android_ar_backgroun" +
            "d_effect_universe,ig_android_biz_story_to_fb_page_improvement,ig_android_direct_continuous_capture,igtv_feed_previews,ig_android_dead_code" +
            "_detection,ig_android_branded_content_tag_redesign_organic,ig_android_direct_view_more_qe,ig_android_flexible_profile_universe,ig_vp9_hd_b" +
            "lacklist,android_ard_ig_download_manager_v2,ig_promote_add_payment_navigation_universe,ig_feed_experience,ig_android_direct_left_aligned_n" +
            "avigation_bar,ig_android_li_session_chaining,ig_android_igtv_watch_later,ig_android_direct_import_google_photos2,invite_friends_by_messeng" +
            "er_in_setting_universe,ig_android_promotion_manager_migration_universe,ig_android_effect_gallery_post_capture_universe,ig_android_smplt_un" +
            "iverse,ig_android_feed_defer_on_interactions,ig_android_direct_state_observer,ig_android_profile_lazy_load_carousel_media,ig_android_direc" +
            "t_reshare_chaining,ig_android_promotion_insights_bloks,ig_direct_max_participants,ig_android_direct_new_gallery,ig_android_flexible_contac" +
            "t_and_category_for_creators,ig_android_stories_video_seeking_audio_bug_fix,ig_android_stories_music_overlay,ig_camera_fast_tti_universe,ig" +
            "_android_wait_for_app_initialization_on_push_action_universe,ig_disable_fsync_universe,ig_android_profile_ppr,ig_android_self_profile_sugg" +
            "est_business_gating,ig_search_hashtag_content_advisory_remove_snooze,ig_android_jp_line_share_universe,ig_android_video_cached_bandwidth_e" +
            "stimate,ig_biz_post_approval_nux_universe,ig_promote_media_picker_universe,ig_android_vc_direct_inbox_ongoing_pill_universe,ig_android_cam" +
            "era_effects_order_universe,ig_android_specific_story_sharing,ig_android_save_to_collections_flow,saved_collections_cache_universe,ig_traff" +
            "ic_routing_universe,ig_android_contact_point_upload_rate_limit_killswitch,ig_android_interactions_nav_to_permalink_followup_universe,ig_an" +
            "droid_create_page_on_top_universe,ig_stories_selfie_sticker,ig_android_gallery_grid_controller_folder_cache,ig_android_live_use_rtc_upload" +
            "_universe,ig_android_video_upload_quality_qe1,ig_android_stories_music_awareness_universe,ig_android_insights_post_dismiss_button,ig_jp_ho" +
            "ldout_2019_h2,ig_android_watch_and_more_redesign,ig_android_story_bottom_sheet_top_clips_mas,ig_android_publisher_stories_migration,ig_and" +
            "roid_xposting_newly_fbc_people,ig_camera_android_share_effect_link_universe,ig_carousel_bumped_organic_impression_client_universe,ig_andro" +
            "id_story_bottom_sheet_music_mas,ig_android_profile_cta_v3,ig_android_follow_request_button_new_ui,ig_shopping_size_selector_redesign,ig_an" +
            "droid_external_gallery_import_affordance,ig_android_explore_lru_cache,ufi_share,ig_android_personal_user_xposting_destination_fix,ig_andro" +
            "id_own_profile_sharing_universe,ig_android_sso_kototoro_app_universe,ig_android_igtv_browse_long_press,ig_android_secondary_inbox_universe" +
            ",ig_android_direct_message_reactions,ig_android_video_raven_streaming_upload_universe,ig_android_stories_question_sticker_music_format,ig_" +
            "android_direct_text_shim_viewholder,ig_android_xposting_reel_memory_share_universe,ig_camera_android_gyro_senser_sampling_period_universe," +
            "ig_android_fs_creation_flow_tweaks,ig_android_view_info_universe,ig_payment_checkout_cvv,ig_android_jp_reel_tray_location_sticker_text_uni" +
            "verse,ig_android_camera_stopmotion,ig_android_camera_leak,ig_threads_app_close_friends_integration,ig_android_page_claim_deeplink_qe,ig_an" +
            "droid_live_webrtc_livewith_params,ig_android_comment_warning_non_english_universe,ig_android_direct_mark_as_read_notif_action,instagram_sh" +
            "opping_hero_carousel_visual_variant_consolidation,ig_threads_clear_notifications_on_has_seen,ig_android_save_collaborative_collections,ig_" +
            "android_partial_share_sheet,ig_android_big_foot_foregroud_reporting,ig_android_video_upload_hevc_encoding_universe,ig_android_vc_migrate_t" +
            "o_bluetooth_v2_universe,ig_android_lock_unlock_dex,ig_android_scroll_main_feed,ig_android_vc_sounds_universe,ig_android_hec_promote_univer" +
            "se,ig_android_igtv_refresh_tv_guide_interval,ig_camera_android_bg_processor,ig_android_experimental_onetap_dialogs_universe,ig_android_sto" +
            "ries_viewer_prefetch_improvements,ig_android_prefetch_logic_infra,ig_android_qr_code_scanner,ig_android_stories_ssi_interstitial_qe,ig_and" +
            "roid_video_qp_logger_universe,ig_android_direct_segmented_video,ig_android_stories_remote_sticker_search,ig_android_feed_ads_ppr_universe," +
            "ig_android_direct_aggregated_media_and_reshares,ig_rn_branded_content_settings_approval_on_select_save,ig_android_shopping_signup_redesign" +
            "_universe,ig_android_recyclerview_binder_group_enabled_universe,ig_android_action_sheet_migration_universe,ig_android_viewmaster_ar_memory" +
            "_improvements,ig_android_explore_use_shopping_endpoint,ig_android_igtv_pip,ig_android_arengine_remote_scripting_universe,ig_android_vc_web" +
            "rtc_params,ig_android_jit,ig_android_video_product_specific_abr,ig_android_promote_content_change_universe,ig_android_remove_unfollow_dial" +
            "og_universe,ig_android_stories_viewer_modal_activity,ig_android_direct_wellbeing_message_reachability_settings,native_contact_invites_univ" +
            "erse,ig_android_stories_layout_universe,ig_android_feed_post_sticker,ig_android_render_output_surface_timeout_universe,ig_android_explore_" +
            "recyclerview_universe,ig_android_stories_discussion_sticker_universe,ig_android_nametag_save_experiment_universe,ig_branded_content_settin" +
            "gs_unsaved_changes_dialog,ig_android_photo_invites,ig_android_direct_selfie_stickers,ig_android_stories_viewer_tall_android_cap_media_univ" +
            "erse,ig_branded_content_tagging_upsell,ig_android_stories_gallery_sticker_universe,android_ig_cameracore_aspect_ratio_fix,ig_android_growt" +
            "h_fci_team_holdout_universe,ig_android_insights_holdout,ig_camera_android_async_effect_api_parsing_universe,ig_android_discover_interests_" +
            "universe,ig_xposting_mention_reshare_stories,ig_android_igtv_ssim_report,ig_close_friends_home_v2,ig_pacing_overriding_universe,ig_android" +
            "_wellbeing_timeinapp_v1_universe,ig_android_webrtc_encoder_factory_universe,ig_promote_prefill_destination_universe,ig_android_media_remod" +
            "el,ig_android_video_raven_passthrough,ig_camera_android_segmentation_v106_igdjango_universe";

        #endregion

        public static string ACCEPT_LANGUAGE = "en-US";

        public const string FACEBOOK_LOGIN_URI = "https://m.facebook.com/v2.3/dialog/oauth?access_token=&client_id=124024574287414&e2e={0}&scope=email&default_audience=friends&redirect_uri=fbconnect://success&display=touch&response_type=token,signed_request&return_scopes=true";
        public const string FACEBOOK_TOKEN = "https://graph.facebook.com/me?access_token={0}&fields=id,is_employee,name";
        public const string FACEBOOK_TOKEN_PICTURE = "https://graph.facebook.com/me?access_token={0}&fields=picture";

        public const string FACEBOOK_USER_AGENT = "Mozilla/5.0 (Linux; Android {0}; {1} Build/{2}; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/69.0.3497.100 Mobile Safari/537.36";
        public const string FACEBOOK_USER_AGENT_DEFAULT = "Mozilla/5.0 (Linux; Android 7.0; PRA-LA1 Build/HONORPRA-LA1; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/69.0.3497.100 Mobile Safari/537.36";

        public const string WEB_USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 OPR/57.0.3098.116";

        public const string ERROR_OCCURRED = "Oops, an error occurred";

        public static readonly Uri BaseInstagramUri = new Uri(BASE_INSTAGRAM_API_URL);

        #endregion Main

        #region Account endpoints constants

        public const string ACCOUNTS_2FA_LOGIN = API_SUFFIX + "/accounts/two_factor_login/";
        public const string ACCOUNTS_2FA_LOGIN_AGAIN = API_SUFFIX + "/accounts/send_two_factor_login_sms/";
        public const string ACCOUNTS_CHANGE_PROFILE_PICTURE = API_SUFFIX + "/accounts/change_profile_picture/";
        public const string ACCOUNTS_CHECK_PHONE_NUMBER = API_SUFFIX + "/accounts/check_phone_number/";
        public const string ACCOUNTS_CONTACT_POINT_PREFILL = API_SUFFIX + "/accounts/contact_point_prefill/";
        public const string ACCOUNTS_CREATE = API_SUFFIX + "/accounts/create/";
        public const string ACCOUNTS_CREATE_VALIDATED = API_SUFFIX + "/accounts/create_validated/";
        public const string ACCOUNTS_DISABLE_SMS_TWO_FACTOR = API_SUFFIX + "/accounts/disable_sms_two_factor/";
        public const string ACCOUNTS_EDIT_PROFILE = API_SUFFIX + "/accounts/edit_profile/";
        public const string ACCOUNTS_ENABLE_SMS_TWO_FACTOR = API_SUFFIX + "/accounts/enable_sms_two_factor/";
        public const string ACCOUNTS_GET_COMMENT_FILTER = API_SUFFIX + "/accounts/get_comment_filter/";
        public const string ACCOUNTS_LOGIN = API_SUFFIX + "/accounts/login/";
        public const string ACCOUNTS_LOGOUT = API_SUFFIX + "/accounts/logout/";
        public const string ACCOUNTS_READ_MSISDN_HEADER = API_SUFFIX + "/accounts/read_msisdn_header/";
        public const string ACCOUNTS_REGEN_BACKUP_CODES = API_SUFFIX + "/accounts/regen_backup_codes/";
        public const string ACCOUNTS_REMOVE_PROFILE_PICTURE = API_SUFFIX + "/accounts/remove_profile_picture/";
        public const string ACCOUNTS_REQUEST_PROFILE_EDIT = API_SUFFIX + "/accounts/current_user/?edit=true";
        public const string ACCOUNTS_SECURITY_INFO = API_SUFFIX + "/accounts/account_security_info/";
        public const string ACCOUNTS_SEND_CONFIRM_EMAIL = API_SUFFIX + "/accounts/send_confirm_email/";
        public const string ACCOUNTS_SEND_RECOVERY_EMAIL = API_SUFFIX + "/accounts/send_recovery_flow_email/";
        public const string ACCOUNTS_SEND_SIGNUP_SMS_CODE = API_SUFFIX + "/accounts/send_signup_sms_code/";
        public const string ACCOUNTS_SEND_SMS_CODE = API_SUFFIX + "/accounts/send_sms_code/";
        public const string ACCOUNTS_SEND_TWO_FACTOR_ENABLE_SMS = API_SUFFIX + "/accounts/send_two_factor_enable_sms/";
        public const string ACCOUNTS_SET_BIOGRAPHY = API_SUFFIX + "/accounts/set_biography/";
        public const string ACCOUNTS_SET_PHONE_AND_NAME = API_SUFFIX + "/accounts/set_phone_and_name/";
        public const string ACCOUNTS_SET_PRESENCE_DISABLED = API_SUFFIX + "/accounts/set_presence_disabled/";
        public const string ACCOUNTS_UPDATE_BUSINESS_INFO = API_SUFFIX + "/accounts/update_business_info/";
        public const string ACCOUNTS_USERNAME_SUGGESTIONS = API_SUFFIX + "/accounts/username_suggestions/";
        public const string ACCOUNTS_VALIDATE_SIGNUP_SMS_CODE = API_SUFFIX + "/accounts/validate_signup_sms_code/";
        public const string ACCOUNTS_VERIFY_SMS_CODE = API_SUFFIX + "/accounts/verify_sms_code/";
        public const string CHANGE_PASSWORD = API_SUFFIX + "/accounts/change_password/";
        public const string CURRENTUSER = API_SUFFIX + "/accounts/current_user/?edit=true";
        public const string SET_ACCOUNT_PRIVATE = API_SUFFIX + "/accounts/set_private/";
        public const string SET_ACCOUNT_PUBLIC = API_SUFFIX + "/accounts/set_public/";
        public const string ACCOUNTS_CONVERT_TO_PERSONAL = API_SUFFIX + "/accounts/convert_to_personal/";
        public const string ACCOUNTS_CREATE_BUSINESS_INFO = API_SUFFIX + "/accounts/create_business_info/";
        public const string ACCOUNTS_GET_PRESENCE = API_SUFFIX + "/accounts/get_presence_disabled/";
        public const string ACCOUNTS_GET_BLOCKED_COMMENTERS = API_SUFFIX + "/accounts/get_blocked_commenters/";
        public const string ACCOUNTS_SET_BLOCKED_COMMENTERS = API_SUFFIX + "/accounts/set_blocked_commenters/";
        public const string ACCOUNTS_REMOVE_TRUSTED_DEVICE = API_SUFFIX + "/accounts/remove_trusted_device/";

        #endregion Account endpoint constants

        #region Business endpoints constants

        /// <summary>
        /// /api/v1/business/instant_experience/get_ix_partners_bundle/?signed_body=b941ff07b83716087710019790b3529ab123c8deabfb216e056651e9cf4b4ca7.{}&ig_sig_key_version=4
        /// </summary>
        public const string BUSINESS_INSTANT_EXPERIENCE = API_SUFFIX + "/business/instant_experience/get_ix_partners_bundle/?signed_body={0}&ig_sig_key_version={1}";

        public const string BUSINESS_SET_CATEGORY = API_SUFFIX + "/business/account/set_business_category/";
        public const string BUSINESS_VALIDATE_URL = API_SUFFIX + "/business/instant_experience/ix_validate_url/";
        public const string BUSINESS_BRANDED_GET_SETTINGS = API_SUFFIX + "/business/branded_content/get_whitelist_settings/";
        public const string BUSINESS_BRANDED_USER_SEARCH = API_SUFFIX + "/users/search/?q={0}&count={1}&branded_content_creator_only=true";
        public const string BUSINESS_BRANDED_UPDATE_SETTINGS = API_SUFFIX + "/business/branded_content/update_whitelist_settings/";
        public const string BUSINESS_CONVERT_TO_BUSINESS_ACCOUNT = API_SUFFIX + "/business_conversion/get_business_convert_social_context/";

        #endregion Business endpoints constants

        #region Collection endpoints constants

        public const string COLLECTION_CREATE_MODULE = API_SUFFIX + "/collection_create/";
        public const string CREATE_COLLECTION = API_SUFFIX + "/collections/create/";
        public const string DELETE_COLLECTION = API_SUFFIX + "/collections/{0}/delete/";
        public const string EDIT_COLLECTION = API_SUFFIX + "/collections/{0}/edit/";
        public const string FEED_SAVED_ADD_TO_COLLECTION_MODULE = "feed_saved_add_to_collection/";
        public const string GET_LIST_COLLECTIONS = API_SUFFIX + "/collections/list/";

        #endregion Collection endpoints constants

        #region Consent endpoints constants

        public const string CONSENT_NEW_USER_FLOW = API_SUFFIX + "/consent/new_user_flow/";
        public const string CONSENT_NEW_USER_FLOW_BEGINS = API_SUFFIX + "/consent/new_user_flow_begins/";
        public const string CONSENT_EXISTING_USER_FLOW = API_SUFFIX + "/consent/existing_user_flow/";



        #endregion Consent endpoints constants

        #region Direct endpoints constants

        public const string DIRECT_BROADCAST_CONFIGURE_VIDEO = API_SUFFIX + "/direct_v2/threads/broadcast/configure_video/";
        public const string DIRECT_BROADCAST_CONFIGURE_PHOTO = API_SUFFIX + "/direct_v2/threads/broadcast/configure_photo/";
        public const string DIRECT_BROADCAST_LINK = API_SUFFIX + "/direct_v2/threads/broadcast/link/";
        public const string DIRECT_BROADCAST_THREAD_LIKE = API_SUFFIX + "/direct_v2/threads/broadcast/like/";
        public const string DIRECT_BROADCAST_LOCATION = API_SUFFIX + "/direct_v2/threads/broadcast/location/";
        public const string DIRECT_BROADCAST_MEDIA_SHARE = API_SUFFIX + "/direct_v2/threads/broadcast/media_share/?media_type={0}";
        public const string DIRECT_BROADCAST_PROFILE = API_SUFFIX + "/direct_v2/threads/broadcast/profile/";
        public const string DIRECT_BROADCAST_REACTION = API_SUFFIX + "/direct_v2/threads/broadcast/reaction/";
        public const string DIRECT_BROADCAST_REEL_SHARE = API_SUFFIX + "/direct_v2/threads/broadcast/reel_share/?media_type={0}";
        public const string DIRECT_BROADCAST_UPLOAD_PHOTO = API_SUFFIX + "/direct_v2/threads/broadcast/upload_photo/";
        public const string DIRECT_BROADCAST_HASHTAG = API_SUFFIX + "/direct_v2/threads/broadcast/hashtag/";
        public const string DIRECT_BROADCAST_LIVE_VIEWER_INVITE = API_SUFFIX + "/direct_v2/threads/broadcast/live_viewer_invite/";
        public const string DIRECT_BROADCAST_SHARE_VOICE = API_SUFFIX + "/direct_v2/threads/broadcast/share_voice/";
        public const string DIRECT_BROADCAST_ANIMATED_MEDIA = API_SUFFIX + "/direct_v2/threads/broadcast/animated_media/";
        public const string DIRECT_BROADCAST_FELIX_SHARE = API_SUFFIX + "/direct_v2/threads/broadcast/felix_share/";
        public const string DIRECT_BROADCAST_REEL_REACT = API_SUFFIX + "/direct_v2/threads/broadcast/reel_react/";

        /// <summary>
        /// post data:
        /// <para>use_unified_inbox=true</para>
        /// <para>recipient_users= user ids split with comma.: ["user id1","user id2","..."]</para>
        /// </summary>
        public const string DIRECT_CREATE_GROUP = API_SUFFIX + "/direct_v2/create_group_thread/";

        public const string DIRECT_PRESENCE = API_SUFFIX + "/direct_v2/get_presence/";
        public const string DIRECT_SHARE = API_SUFFIX + "/direct_share/inbox/";
        public const string DIRECT_STAR = API_SUFFIX + "/direct_v2/threads/{0}/label/";
        public const string DIRECT_THREAD_HIDE = API_SUFFIX + "/direct_v2/threads/{0}/hide/";
        public const string DIRECT_THREAD_ADD_USER = API_SUFFIX + "/direct_v2/threads/{0}/add_user/";
        /// <summary>
        ///  deprecated
        /// </summary>
        public const string DIRECT_THREAD_ITEM_SEEN = API_SUFFIX + "/direct_v2/visual_threads/{0}/item_seen/";
        public const string DIRECT_THREAD_SEEN = API_SUFFIX + "/direct_v2/threads/{0}/items/{1}/seen/";
        public const string DIRECT_THREAD_LEAVE = API_SUFFIX + "/direct_v2/threads/{0}/leave/";
        public const string DIRECT_THREAD_MESSAGES_MUTE = API_SUFFIX + "/direct_v2/threads/{0}/mute/";
        public const string DIRECT_THREAD_MESSAGES_UNMUTE = API_SUFFIX + "/direct_v2/threads/{0}/unmute/";
        public const string DIRECT_THREAD_UPDATE_TITLE = API_SUFFIX + "/direct_v2/threads/{0}/update_title/";
        public const string DIRECT_UNSTAR = API_SUFFIX + "/direct_v2/threads/{0}/unlabel/";
        public const string GET_DIRECT_INBOX = API_SUFFIX + "/direct_v2/inbox/";
        public const string GET_DIRECT_PENDING_INBOX = API_SUFFIX + "/direct_v2/pending_inbox/";
        public const string GET_DIRECT_SHARE_USER = API_SUFFIX + "/direct_v2/threads/broadcast/profile/";
        public const string GET_DIRECT_TEXT_BROADCAST = API_SUFFIX + "/direct_v2/threads/broadcast/text/";
        public const string GET_DIRECT_THREAD = API_SUFFIX + "/direct_v2/threads/{0}/";
        public const string GET_DIRECT_THREAD_APPROVE = API_SUFFIX + "/direct_v2/threads/{0}/approve/";
        public const string GET_DIRECT_THREAD_APPROVE_MULTIPLE = API_SUFFIX + "/direct_v2/threads/approve_multiple/";
        public const string GET_DIRECT_THREAD_DECLINE = API_SUFFIX + "/direct_v2/threads/{0}/decline/";
        public const string GET_DIRECT_THREAD_DECLINE_MULTIPLE = API_SUFFIX + "/direct_v2/threads/decline_multiple/";
        public const string GET_DIRECT_THREAD_DECLINEALL = API_SUFFIX + "/direct_v2/threads/decline_all/";
        public const string GET_PARTICIPANTS_RECIPIENT_USER = API_SUFFIX + "/direct_v2/threads/get_by_participants/?recipient_users=[{0}]";
        public const string GET_RANK_RECIPIENTS_BY_USERNAME = API_SUFFIX + "/direct_v2/ranked_recipients/?mode={1}&show_threads=true&query={0}";
        public const string GET_RANKED_RECIPIENTS = API_SUFFIX + "/direct_v2/ranked_recipients/";
        public const string GET_RECENT_RECIPIENTS = API_SUFFIX + "/direct_share/recent_recipients/";
        public const string STORY_SHARE = API_SUFFIX + "/direct_v2/threads/broadcast/story_share/?media_type={0}";
        public const string DIRECT_THREAD_DELETE_MESSAGE = API_SUFFIX + "/direct_v2/threads/{0}/items/{1}/delete/"; 

        #endregion Direct endpoints constants

        #region Discover endpoints constants

        public const string DISCOVER_AYML = API_SUFFIX + "/discover/ayml/";
        public const string DISCOVER_CHAINING = API_SUFFIX + "/discover/chaining/?target_id={0}";
        public const string DISCOVER_EXPLORE = API_SUFFIX + "/discover/explore/";
        public const string DISCOVER_TOPICAL_EXPLORE = API_SUFFIX + "/discover/topical_explore/";
        public const string DISCOVER_FETCH_SUGGESTION_DETAILS = API_SUFFIX + "/discover/fetch_suggestion_details/?target_id={0}&chained_" +
                                                                            "ids={1}&media_info_count=0&include_social_context=1&use_full_media_info=0";
        public const string DISCOVER_TOP_LIVE = API_SUFFIX + "/discover/top_live/";
        public const string DISCOVER_TOP_LIVE_STATUS = API_SUFFIX + "/discover/top_live_status/";
        public const string DISCOVER_DISMISS_SUGGESTION = API_SUFFIX + "/discover/dismiss_suggestion/";
        public const string DISCOVER_EXPLORE_REPORT = API_SUFFIX + "/discover/explore_report/";


        public const string DISCOVER_SURFACE_WITH_SU = API_SUFFIX + "/discover/surface_with_su/";


        #endregion Discover endpoints constants

        #region FBSearch endpoints constants

        public const string FBSEARCH_CLEAR_SEARCH_HISTORY = API_SUFFIX + "/fbsearch/clear_search_history/";
        public const string FBSEARCH_GET_HIDDEN_SEARCH_ENTITIES = API_SUFFIX + "/fbsearch/get_hidden_search_entities/";

        public const string FBSEARCH_HIDE_SEARCH_ENTITIES = API_SUFFIX + "/fbsearch/hide_search_entities/";

        /// <summary>
        /// get nearby places
        /// </summary>
        public const string FBSEARCH_PLACES = API_SUFFIX + "/fbsearch/places/";
        
        public const string FBSEARCH_PROFILE_SEARCH = API_SUFFIX + "/fbsearch/profile_link_search/?q={0}&count={1}";
        public const string FBSEARCH_RECENT_SEARCHES = API_SUFFIX + "/fbsearch/recent_searches/";
        public const string FBSEARCH_NULLSTATE_DYNAMIC_SECTIONS = API_SUFFIX + "/fbsearch/nullstate_dynamic_sections/?type=blended";
        public const string FBSEARCH_REGISTER_RECENT_SEARCH_CLICK = API_SUFFIX + "/fbsearch/register_recent_search_click/";
        public const string FBSEARCH_ACCOUNTS_RECS = API_SUFFIX + "/fbsearch/accounts_recs/?surface=discover_page&target_user_id={0}";
        public const string FBSEARCH_SUGGESTED_SEARCHES = API_SUFFIX + "/fbsearch/suggested_searches/?type={0}";
        public const string FBSEARCH_TOPSEARCH = API_SUFFIX + "/fbsearch/topsearch/";
        public const string FBSEARCH_TOPSEARCH_FALT = API_SUFFIX + "/fbsearch/topsearch_flat/";
        public const string FBSEARCH_TOPSEARCH_FALT_PARAMETER = API_SUFFIX + "/fbsearch/topsearch_flat/?rank_token={0}&timezone_offset={1}&query={2}&context={3}";

        #endregion FBSearch endpoints constants

        #region FB endpoints constants

        public const string FB_ENTRYPOINT_INFO = API_SUFFIX + "/fb/fb_entrypoint_info/";
        public const string FB_FACEBOOK_SIGNUP = API_SUFFIX + "/fb/facebook_signup/";
        public const string FB_GET_INVITE_SUGGESTIONS = API_SUFFIX + "/fb/get_invite_suggestions/";

        #endregion FB endpoints constants

        #region Feed endpoints constants

        public const string FEED_ONLY_ME_FEED = API_SUFFIX + "/feed/only_me_feed/";
        /// <summary>
        /// {0} = rank token <<<<< this endpoint is deprecated
        /// </summary>
        public const string FEED_POPULAR = API_VERSION + "/feed/popular/?people_teaser_supported=1&rank_token={0}&ranked_content=true";

        public const string FEED_PROMOTABLE_MEDIA = API_SUFFIX + "/feed/promotable_media/";
        public const string FEED_REEL_MEDIA = API_SUFFIX + "/feed/reels_media/";
        public const string FEED_SAVED = API_SUFFIX + "/feed/saved/";
        public const string GET_COLLECTION = API_SUFFIX + "/feed/collection/{0}/";
        public const string GET_STORY_TRAY = API_SUFFIX + "/feed/reels_tray/";
        public const string GET_TAG_FEED = API_SUFFIX + "/feed/tag/{0}/";
        public const string GET_USER_STORY = API_SUFFIX + "/feed/user/{0}/reel_media/";
        public const string GET_USER_TAGS = API_SUFFIX + "/usertags/{0}/feed/";
        public const string LIKE_FEED = API_SUFFIX + "/feed/liked/";
        public const string TIMELINEFEED = API_SUFFIX + "/feed/timeline/";
        public const string USER_REEL_FEED = API_SUFFIX + "/feed/user/{0}/reel_media/";
        public const string USEREFEED = API_SUFFIX + "/feed/user/";
        public const string USER_FEED_CAPABILITIES = API_SUFFIX + "/feed/user/{0}/story/";

        #endregion Feed endpoints constants

        #region Friendship endpoints constants

        public const string FRIENDSHIPS_APPROVE = API_SUFFIX + "/friendships/approve/{0}/";
        public const string FRIENDSHIPS_AUTOCOMPLETE_USER_LIST = API_SUFFIX + "/friendships/autocomplete_user_list/";
        public const string FRIENDSHIPS_BLOCK_USER = API_SUFFIX + "/friendships/block/{0}/";
        public const string FRIENDSHIPS_UNFOLLOW_CHAINING_COUNT = API_SUFFIX + "/friendships/unfollow_chaining_count/{0}/";
  
        public const string FRIENDSHIPS_FOLLOW_USER = API_SUFFIX + "/friendships/create/{0}/";
        public const string FRIENDSHIPS_IGNORE = API_SUFFIX + "/friendships/ignore/{0}/";
        public const string FRIENDSHIPS_RECENT_FOLLOWERS = API_SUFFIX + "/friendships/recent_followers/";


        public const string FRIENDSHIPS_PENDING_REQUESTS = API_SUFFIX + "/friendships/pending/";//?rank_mutual=0&rank_token={0}";
        public const string FRIENDSHIPS_REMOVE_FOLLOWER = API_SUFFIX + "/friendships/remove_follower/{0}/";
        /// <summary>
        /// hide your stories from specific users
        /// </summary>
        public const string FRIENDSHIPS_SET_REEL_BLOCK_STATUS = API_SUFFIX + "/friendships/set_reel_block_status/";

        public const string FRIENDSHIPS_SHOW_MANY = API_SUFFIX + "/friendships/show_many/";


        public const string FRIENDSHIPS_UNBLOCK_USER = API_SUFFIX + "/friendships/unblock/{0}/";


        public const string FRIENDSHIPS_FAVORITE = API_SUFFIX + "/friendships/favorite/{0}/";
        public const string FRIENDSHIPS_UNFAVORITE = API_SUFFIX + "/friendships/unfavorite/{0}/";
        public const string FRIENDSHIPS_FAVORITE_FOR_STORIES = API_SUFFIX + "/friendships/favorite_for_stories/{0}/";
        public const string FRIENDSHIPS_UNFAVORITE_FOR_STORIES = API_SUFFIX + "/friendships/unfavorite_for_stories/{0}/";
        public const string FRIENDSHIPS_UNFOLLOW_USER = API_SUFFIX + "/friendships/destroy/{0}/";
        public const string FRIENDSHIPS_USER_FOLLOWERS = API_SUFFIX + "/friendships/{0}/followers/";
        public const string FRIENDSHIPS_USER_FOLLOWERS_MUTUALFIRST = API_SUFFIX + "/friendships/{0}/followers/?rank_mutual={1}";
        public const string FRIENDSHIPS_USER_FOLLOWING = API_SUFFIX + "/friendships/{0}/following/";
        public const string FRIENDSHIPSTATUS = API_SUFFIX + "/friendships/show/";
        public const string FRIENDSHIPS_MARK_USER_OVERAGE = API_SUFFIX + "/friendships/mark_user_overage/{0}/feed/";
        public const string FRIENDSHIPS_MUTE_POST_STORY = API_SUFFIX + "/friendships/mute_posts_or_story_from_follow/";
        public const string FRIENDSHIPS_UNMUTE_POST_STORY = API_SUFFIX + "/friendships/unmute_posts_or_story_from_follow/";
        public const string FRIENDSHIPS_BLOCK_FRIEND_REEL = API_SUFFIX + "/friendships/block_friend_reel/{0}/";
        public const string FRIENDSHIPS_UNBLOCK_FRIEND_REEL = API_SUFFIX + "/friendships/unblock_friend_reel/{0}/";
        public const string FRIENDSHIPS_MUTE_FRIEND_REEL = API_SUFFIX + "/friendships/mute_friend_reel/{0}/";
        public const string FRIENDSHIPS_UNMUTE_FRIEND_REEL = API_SUFFIX + "/friendships/unmute_friend_reel/{0}/";
        public const string FRIENDSHIPS_BLOCKED_REEL = API_SUFFIX + "/friendships/blocked_reels/";
        public const string FRIENDSHIPS_BESTIES = API_SUFFIX + "/friendships/besties/";
        public const string FRIENDSHIPS_BESTIES_SUGGESTIONS = API_SUFFIX + "/friendships/bestie_suggestions/";
        public const string FRIENDSHIPS_SET_BESTIES = API_SUFFIX + "/friendships/set_besties/";

        #endregion Friendships endpoints constants

        #region Graphql, insights [statistics] endpoints constants

        public const string GRAPH_QL = API_SUFFIX + "/ads/graphql/";
        public const string GRAPH_QL_STATISTICS = GRAPH_QL + "?locale={0}&vc_policy=insights_policy&surface={1}";
        public const string INSIGHTS_MEDIA = API_SUFFIX + "/insights/account_organic_insights/?show_promotions_in_landing_page=true&first={0}";
        public const string INSIGHTS_MEDIA_SINGLE = API_SUFFIX + "/insights/media_organic_insights/{0}?{1}={2}";

        #endregion Graphql, insights [statistics] endpoints constants

        #region Highlight endpoints constants

        public const string HIGHLIGHT_CREATE_REEL = API_SUFFIX + "/highlights/create_reel/";
        public const string HIGHLIGHT_DELETE_REEL = API_SUFFIX + "/highlights/{0}/delete_reel/";
        public const string HIGHLIGHT_EDIT_REEL = API_SUFFIX + "/highlights/{0}/edit_reel/";
        public const string HIGHLIGHT_TRAY = API_SUFFIX + "/highlights/{0}/highlights_tray/";

        #endregion Highlight endpoints constants

        #region IgTv (instagram tv) endpoints constants

        public const string IGTV_CHANNEL = API_SUFFIX + "/igtv/channel/";
        public const string IGTV_SEARCH = API_SUFFIX + "/igtv/search/?query={0}";
        public const string IGTV_SUGGESTED_SEARCHES = API_SUFFIX + "/igtv/suggested_searches/?query=";
        public const string IGTV_TV_GUIDE = API_SUFFIX + "/igtv/tv_guide/";
        public const string MEDIA_CONFIGURE_TO_IGTV = API_SUFFIX + "/media/configure_to_igtv/?video=1";
        public const string IGTV_BROWSE_FEED = API_SUFFIX + "/igtv/browse_feed/";
        public const string IGTV_WRITE_SEEN = API_SUFFIX + "/igtv/write_seen_state/";

        #endregion IgTv (instagram tv) endpoints constants

        #region Language endpoints constants

        public const string LANGUAGE_TRANSLATE = API_SUFFIX + "/language/translate/?id={0}&type=3";
        public const string LANGUAGE_TRANSLATE_COMMENT = API_SUFFIX + "/language/bulk_translate/?comment_ids={0}";

        #endregion Language endpoints constants

        #region Live endpoints constants

        public const string LIVE_ADD_TO_POST_LIVE = API_SUFFIX + "/live/{0}/add_to_post_live/";
        public const string LIVE_COMMENT = API_SUFFIX + "/live/{0}/comment/";
        public const string LIVE_CREATE = API_SUFFIX + "/live/create/";
        public const string LIVE_DELETE_POST_LIVE = API_SUFFIX + "/live/{0}/delete_post_live/";
        public const string LIVE_END = API_SUFFIX + "/live/{0}/end_broadcast/";
        public const string LIVE_GET_COMMENT = API_SUFFIX + "/live/{0}/get_comment/";
        public const string LIVE_GET_COMMENT_LASTCOMMENTTS = API_SUFFIX + "/live/{0}/get_comment/?last_comment_ts={1}";
        public const string LIVE_GET_FINAL_VIEWER_LIST = API_SUFFIX + "/live/{0}/get_final_viewer_list/";
        public const string LIVE_GET_JOIN_REQUESTS = API_SUFFIX + "/live/{0}/get_join_requests/";
        public const string LIVE_GET_LIKE_COUNT = API_SUFFIX + "/live/{0}/get_like_count/";
        public const string LIVE_GET_LIVE_PRESENCE = API_SUFFIX + "/live/get_live_presence/?presence_type=30min";
        public const string LIVE_GET_POST_LIVE_COMMENT = API_SUFFIX + "/live/{0}/get_post_live_comments/?starting_offset={1}&encoding_tag={2}";
        public const string LIVE_GET_POST_LIVE_VIEWERS_LIST = API_SUFFIX + "/live/{0}/get_post_live_viewers_list/";
        public const string LIVE_GET_SUGGESTED_BROADCASTS = API_SUFFIX + "/live/get_suggested_broadcasts/";
        public const string LIVE_GET_VIEWER_LIST = API_SUFFIX + "/live/{0}/get_viewer_list/";
        public const string LIVE_HEARTBEAT_AND_GET_VIEWER_COUNT = API_SUFFIX + "/live/{0}/heartbeat_and_get_viewer_count/";
        public const string LIVE_INFO = API_SUFFIX + "/live/{0}/info/";
        public const string LIVE_LIKE = API_SUFFIX + "/live/{0}/like/";
        public const string LIVE_MUTE_COMMENTS = API_SUFFIX + "/live/{0}/mute_comment/";
        public const string LIVE_PIN_COMMENT = API_SUFFIX + "/live/{0}/pin_comment/";
        public const string LIVE_POST_LIVE_LIKES = API_SUFFIX + "/live/{0}/get_post_live_likes/?starting_offset={1}&encoding_tag={2}";
        public const string LIVE_START = API_SUFFIX + "/live/{0}/start/";
        public const string LIVE_UNMUTE_COMMENTS = API_SUFFIX + "/live/{0}/unmute_comment/";
        public const string LIVE_UNPIN_COMMENT = API_SUFFIX + "/live/{0}/unpin_comment/";

        #endregion Live endpoints constants

        #region Location endpoints constants
        /// <summary>
        /// It seems deprecated and can't get feeds, only stories will recieve
        /// </summary>
        public const string LOCATION_FEED = API_SUFFIX + "/feed/location/{0}/";
        public const string LOCATION_SECTION = API_SUFFIX + "/locations/{0}/sections/";

        public const string LOCATION_SEARCH = API_SUFFIX + "/location_search/";

        public const string LOCATIONS_INFO = API_SUFFIX + "/locations/{0}/info/";
        /// <summary>
        /// {0} => external id, NOT WORKING
        /// </summary>
        public const string LOCATIONS_RELEATED = API_SUFFIX + "/locations/{0}/related/";

        #endregion Location endpoints constants

        #region Media endpoints constants

        public const string ALLOW_MEDIA_COMMENTS = API_SUFFIX + "/media/{0}/enable_comments/";
        public const string DELETE_COMMENT = API_SUFFIX + "/media/{0}/comment/{1}/delete/";
        public const string DELETE_MEDIA = API_SUFFIX + "/media/{0}/delete/?media_type={1}";
        public const string DELETE_MULTIPLE_COMMENT = API_SUFFIX + "/media/{0}/comment/bulk_delete/";
        public const string DISABLE_MEDIA_COMMENTS = API_SUFFIX + "/media/{0}/disable_comments/";
        public const string EDIT_MEDIA = API_SUFFIX + "/media/{0}/edit_media/";
        public const string GET_MEDIA = API_SUFFIX + "/media/{0}/info/";
        public const string GET_SHARE_LINK = API_SUFFIX + "/media/{0}/permalink/?share_to_app=copy_link";
        public const string LIKE_COMMENT = API_SUFFIX + "/media/{0}/comment_like/";
        public const string LIKE_MEDIA = API_SUFFIX + "/media/{0}/like/";
        public const string MAX_MEDIA_ID_POSTFIX = "/media/?max_id=";
        public const string MEDIA = "/media/";
        public const string MEDIA_ALBUM_CONFIGURE = API_SUFFIX + "/media/configure_sidecar/";
        public const string MEDIA_COMMENT_LIKERS = API_SUFFIX + "/media/{0}/comment_likers/";
        public const string MEDIA_COMMENTS = API_SUFFIX + "/media/{0}/comments/";//?can_support_threading=true";
        public const string MEDIA_CONFIGURE = API_SUFFIX + "/media/configure/";
        public const string MEDIA_CONFIGURE_VIDEO = API_SUFFIX + "/media/configure/?video=1";
        public const string MEDIA_UPLOAD_FINISH_VIDEO = API_SUFFIX + "/media/upload_finish/?video=1";
        public const string MEDIA_UPLOAD_FINISH = API_SUFFIX + "/media/upload_finish/";
        public const string MEDIA_INFOS = API_SUFFIX + "/media/infos/?_uuid={0}&_csrftoken={1}&media_ids={2}&ranked_content=true&include_inactive_reel=true";
        public const string MEDIA_CONFIGURE_NAMETAG = API_SUFFIX + "/media/configure_to_nametag/";
        public const string MEDIA_INLINE_COMMENTS = API_SUFFIX + "/media/{0}/comments/{1}/inline_child_comments/";
        public const string MEDIA_LIKERS = API_SUFFIX + "/media/{0}/likers/";
        public const string MEDIA_REPORT = API_SUFFIX + "/media/{0}/flag_media/";
        public const string MEDIA_REPORT_COMMENT = API_SUFFIX + "/media/{0}/comment/{1}/flag/";
        public const string MEDIA_SAVE = API_SUFFIX + "/media/{0}/save/";
        public const string MEDIA_UNSAVE = API_SUFFIX + "/media/{0}/unsave/";

        public const string MEDIA_VALIDATE_REEL_URL = API_SUFFIX + "/media/validate_reel_url/";
        public const string POST_COMMENT = API_SUFFIX + "/media/{0}/comment/";
        public const string SEEN_MEDIA = API_SUFFIX + "/media/seen/";
        public const string SEEN_MEDIA_STORY = API_SUFFIX_V2 + "/media/seen/?reel=1&live_vod=0";
        public const string STORY_CONFIGURE = API_SUFFIX + "/media/configure_to_reel/";
        public const string STORY_CONFIGURE_VIDEO = API_SUFFIX + "/media/configure_to_story/?video=1";
        public const string STORY_CONFIGURE_VIDEO2 = API_SUFFIX + "/media/configure_to_story/";
        public const string STORY_MEDIA_INFO_UPLOAD = API_SUFFIX + "/media/mas_opt_in_info/";
        public const string UNLIKE_COMMENT = API_SUFFIX + "/media/{0}/comment_unlike/";
        public const string UNLIKE_MEDIA = API_SUFFIX + "/media/{0}/unlike/";
        public const string MEDIA_STORY_VIEWERS = API_SUFFIX + "/media/{0}/list_reel_media_viewer/";
        public const string MEDIA_BLOCKED = API_SUFFIX + "/media/blocked/";
        public const string MEDIA_ARCHIVE = API_SUFFIX + "/media/{0}/only_me/";
        public const string MEDIA_UNARCHIVE = API_SUFFIX + "/media/{0}/undo_only_me/";
        public const string MEDIA_STORY_POLL_VOTERS = API_SUFFIX + "/media/{0}/{1}/story_poll_voters/";
        public const string MEDIA_STORY_POLL_VOTE = API_SUFFIX + "/media/{0}/{1}/story_poll_vote/";
        public const string MEDIA_STORY_SLIDER_VOTE = API_SUFFIX + "/media/{0}/{1}/story_slider_vote/";
        public const string MEDIA_STORY_QUESTION_RESPONSE = API_SUFFIX + "/media/{0}/{1}/story_question_response/";
        public const string MEDIA_STORY_COUNTDOWNS = API_SUFFIX + "/media/story_countdowns/";
        public const string MEDIA_FOLLOW_COUNTDOWN = API_SUFFIX + "/media/{0}/follow_story_countdown/";
        public const string MEDIA_UNFOLLOW_COUNTDOWN = API_SUFFIX + "/media/{0}/unfollow_story_countdown/";



        public const string MEDIA_TAG = API_SUFFIX + "/media/{0}/tags/";
        public const string MEDIA_STORY_CHAT_REQUEST = API_SUFFIX + "/media/story_chat_request/";
        public const string MEDIA_STORY_CHAT_CANCEL_REQUEST = API_SUFFIX + "/media/story_chat_cancel_request/";

        #endregion Media endpoints constants

        #region News endpoints constants

        public const string GET_FOLLOWING_RECENT_ACTIVITY = API_SUFFIX + "/news/";
        public const string GET_RECENT_ACTIVITY = API_SUFFIX + "/news/inbox/";
        /// <summary>
        /// post params:
        /// <para>"action":"click"</para>
        /// </summary>
        public const string NEWS_LOG = API_SUFFIX + "/news/log/";

        #endregion News endpoints constants

        #region Notification endpoints constants

        public const string NOTIFICATION_BADGE = API_SUFFIX + "/notifications/badge/";
        public const string PUSH_REGISTER = API_SUFFIX + "/push/register/";

        #endregion Notification endpoints constants

        #region Shopping endpoints constants

        public const string USER_SHOPPABLE_MEDIA = API_SUFFIX + "/feed/user/{0}/shoppable_media/";

        public const string COMMERCE_PRODUCT_INFO = API_SUFFIX + "/commerce/products/{0}/?media_id={1}&device_width={2}";

        #endregion Shopping endpoints constants

        #region Tags endpoints constants

        public const string GET_TAG_INFO = API_SUFFIX + "/tags/{0}/info/";
        public const string SEARCH_TAGS = API_SUFFIX + "/tags/search/?q={0}&count={1}";
        public const string TAG_FOLLOW = API_SUFFIX + "/tags/follow/{0}/";
        public const string TAG_RANKED = API_SUFFIX + "/tags/{0}/ranked_sections/";
        public const string TAG_RECENT = API_SUFFIX + "/tags/{0}/recent_sections/";
        public const string TAG_SECTION = API_SUFFIX + "/tags/{0}/sections/";
        /// <summary>
        /// queries:
        /// <para>visited = [{"id":"TAG ID","type":"hashtag"}]</para>
        /// <para>related_types = ["location","hashtag"]</para>
        /// </summary>
        public const string TAG_RELATED = API_SUFFIX + "/tags/{0}/related/";

        public const string TAG_STORY = API_SUFFIX + "/tags/{0}/story/";
        public const string TAG_SUGGESTED = API_SUFFIX + "/tags/suggested/";
        public const string TAG_UNFOLLOW = API_SUFFIX + "/tags/unfollow/{0}/";
        public const string TAG_MEDIA_REPORT = API_SUFFIX + "/tags/hashtag_media_report/";

        public const string TAG_CHANNEL_VIEWER = API_SUFFIX + "/tags/channel_viewer/hashtag_videos/{0}/";


        #endregion Tags endpoints constants

        #region Users endpoints constants

        public const string ACCOUNTS_LOOKUP_PHONE = API_SUFFIX + "/users/lookup_phone/";
        public const string ARLINK_DOWNLOAD_INFO = API_SUFFIX + "/users/arlink_download_info/?version_override=2.2.1";
        public const string GET_USER_INFO_BY_ID = API_SUFFIX + "/users/{0}/info/";
        public const string GET_USER_INFO_BY_USERNAME = API_SUFFIX + "/users/{0}/usernameinfo/";
        public const string SEARCH_USERS = API_SUFFIX + "/users/search/";
        public const string USERS_CHECK_EMAIL = API_SUFFIX + "/users/check_email/";
        public const string USERS_CHECK_USERNAME = API_SUFFIX + "/users/check_username/";
        public const string USERS_LOOKUP = API_SUFFIX + "/users/lookup/";
        public const string USERS_NAMETAG_CONFIG = API_SUFFIX + "/users/nametag_config/";
        public const string USERS_REEL_SETTINGS = API_SUFFIX + "/users/reel_settings/";
        public const string USERS_REPORT = API_SUFFIX + "/users/{0}/flag_user/";
        public const string USERS_SEARCH = API_SUFFIX + "/users/search/?search_surface=user_search_page&timezone_offset={0}&q={1}&count={2}";
        public const string USERS_SET_REEL_SETTINGS = API_SUFFIX + "/users/set_reel_settings/";
        public const string USERS_FOLLOWING_TAG_INFO = API_SUFFIX + "/users/{0}/following_tags_info/";
        public const string USERS_FULL_DETAIL_INFO = API_SUFFIX + "/users/{0}/full_detail_info/";
        public const string USERS_NAMETAG_LOOKUP = API_SUFFIX + "/users/nametag_lookup/";
        public const string USERS_BLOCKED_LIST = API_SUFFIX + "/users/blocked_list/";
        public const string USERS_ACCOUNT_DETAILS = API_SUFFIX + "/users/{0}/account_details/";

        #endregion Users endpoints constants

        #region Upload endpoints constants

        public const string UPLOAD_PHOTO = INSTAGRAM_URL + "/rupload_igphoto/{0}_0_{1}";
        public const string UPLOAD_PHOTO_OLD = API_SUFFIX + "/upload/photo/";
        public const string UPLOAD_VIDEO = INSTAGRAM_URL + "/rupload_igvideo/{0}_0_{1}";
        public const string UPLOAD_VIDEO_OLD = API_SUFFIX + "/upload/video/";

        #endregion Upload endpoints constants

        #region Other endpoints constants

        public const string ADDRESSBOOK_LINK = API_SUFFIX + "/address_book/link/?include=extra_display_name,thumbnails";
        public const string ARCHIVE_REEL_DAY_SHELLS = API_SUFFIX + "/archive/reel/day_shells/?include_cover=0";
        public const string DYI_REQUEST_DOWNLOAD_DATA = API_SUFFIX + "/dyi/request_download_data/";
        public const string DYI_CHECK_DATA_STATE = API_SUFFIX + "/dyi/check_data_state/";
        public const string DYNAMIC_ONBOARDING_GET_STEPS = API_SUFFIX + "/dynamic_onboarding/get_steps/";
        public const string GET_MEDIAID = API_SUFFIX + "/oembed/?url={0}";
        public const string MEGAPHONE_LOG = API_SUFFIX + "/megaphone/log/";
        public const string GET_VIEWABLE_STATUSES = API_SUFFIX + "/status/get_viewable_statuses/";

        public const string QE_EXPOSE = API_SUFFIX + "/qe/expose/";

        public const string CHALLENGE = API_SUFFIX + "/challenge/";

        public const string LAUNCHER_SYNC = API_SUFFIX + "/launcher/sync/";

        #endregion Other endpoints constants

        #region Web endpoints constants

        public static string WEB_ADDRESS = "https://www.instagram.com";
        public static string WEB_ACCOUNTS = "/accounts/";
        public static string WEB_ACCOUNT_DATA = WEB_ACCOUNTS + "access_tool";
        public static string WEB_CURRENT_FOLLOW_REQUESTS = WEB_ACCOUNT_DATA + "/current_follow_requests";
        public static string WEB_FORMER_EMAILS = WEB_ACCOUNT_DATA + "/former_emails";
        public static string WEB_FORMER_PHONES = WEB_ACCOUNT_DATA + "/former_phones";
        public static string WEB_FORMER_USERNAMES = WEB_ACCOUNT_DATA + "/former_usernames";
        public static string WEB_FORMER_FULL_NAMES = WEB_ACCOUNT_DATA + "/former_full_names";
        public static string WEB_FORMER_BIO_TEXTS = WEB_ACCOUNT_DATA + "/former_bio_texts";
        public static string WEB_FORMER_BIO_LINKS = WEB_ACCOUNT_DATA + "/former_links_in_bio";


        public static string WEB_CURSOR = "__a=1&cursor={0}";

        public static readonly Uri InstagramWebUri = new Uri(WEB_ADDRESS);
        #endregion
    }
}